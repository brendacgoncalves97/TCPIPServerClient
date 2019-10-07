using CommonClassLibs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPClient;

namespace TCPIPClient
{
    public partial class frmClient : Form
    {
        /*******************************************************/
        private Client client = null;//Client Socket class

        private MotherOfRawPackets HostServerRawPackets = null;
        static AutoResetEvent autoEventHostServer = null;//mutex
        static AutoResetEvent autoEvent2;//mutex
        private Thread DataProcessHostServerThread = null;
        private Thread FullPacketDataProcessThread = null;
        private Queue<FullPacket> FullHostServerPacketList = null;
        /*******************************************************/

        bool AppIsExiting = false;
        bool ServerConnected = false;
        int MyHostServerID = 0;
        long ServerTime = DateTime.Now.Ticks;

        System.Windows.Forms.Timer GeneralTimer = null;

        public frmClient()
        {
            InitializeComponent();
        }

        private void frmClient_Load(object sender, EventArgs e)
        {
            /**********************************************/
            //Create a directory we can write stuff too
            CheckOnApplicationDirectory();
            /**********************************************/

            pictureBox1.Image = imageListStatusLights.Images["RED"];

            textBoxText.Text = "This is a test connection";
        }

        private void frmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoServerDisconnect();
            AppIsExiting = true;
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            ServerConnected = true;//Set this before initializing the connection loops
            InitializeServerConnection();
            if (ConnectToHostServer())
            {
                ServerConnected = true;
                buttonConnectToServer.Enabled = false;
                buttonDisconnect.Enabled = true;
                buttonSendDataToServer.Enabled = true;
                labelStatusInfo.Text = "Connected!!";
                labelStatusInfo.ForeColor = System.Drawing.Color.Green;
                BeginGeneralTimer();
            }
            else
            {
                ServerConnected = false;
                labelStatusInfo.Text = "Can't connect";
                labelStatusInfo.ForeColor = System.Drawing.Color.Red;
                pictureBox1.Image = imageListStatusLights.Images["RED"];
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            TellServerImDisconnecting();
            DoServerDisconnect();
            buttonDisconnect.Enabled = false;
            buttonSendDataToServer.Enabled = false;
            ServerConnected = false;
            labelStatusInfo.Text = "Disconnected";
            labelStatusInfo.ForeColor = System.Drawing.Color.Red;
            pictureBox1.Image = imageListStatusLights.Images["RED"];
            buttonConnectToServer.Enabled = true;
            SetSomeLabelInfoFromThread("...");
        }

        private void buttonSendDataToServer_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = imageListStatusLights.Images["BLUE"];
            PACKET_DATA xdata = new PACKET_DATA();

            /****************************************************************/
            //prepair the start packet
            xdata.Packet_Type = (UInt16)PACKETTYPES.TYPE_Message;
            xdata.Data_Type = (UInt16)PACKETTYPES_SUBMESSAGE.SUBMSG_MessageStart;
            xdata.Packet_Size = 16;
            xdata.maskTo = 0;
            xdata.idTo = 0;
            xdata.idFrom = 0;

            //Before we send the text, lets stuff those Number values in the first data packet!
            Int32 num1 = 0;
            Int32.TryParse(textBoxNum1.Text, out num1);
            xdata.Data16 = num1;
            Int32 num2 = 0;
            Int32.TryParse(textBoxNum2.Text, out num2);
            xdata.Data17 = num2;

            int pos = 0;
            int chunkSize = xdata.szStringDataA.Length;//300 bytes

            if (textBoxText.Text.Length <= xdata.szStringDataA.Length)
            {
                textBoxText.Text.CopyTo(0, xdata.szStringDataA, 0, textBoxText.Text.Length);
                chunkSize = textBoxText.Text.Length;
            }
            else
                textBoxText.Text.CopyTo(0, xdata.szStringDataA, 0, xdata.szStringDataA.Length);

            xdata.Data1 = (UInt32)chunkSize;

            byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

            SendMessageToServer(byData);

            /**************************************************/
            //Send the message body(if there is any)
            xdata.Data_Type = (UInt16)PACKETTYPES_SUBMESSAGE.SUBMSG_MessageGuts;
            pos = chunkSize;//set position
            while (true)
            {
                int PosFromEnd = textBoxText.Text.Length - pos;

                if (PosFromEnd <= 0)
                    break;

                Array.Clear(xdata.szStringDataA, 0, xdata.szStringDataA.Length);//Clear this field before putting more data in it

                if (PosFromEnd < xdata.szStringDataA.Length)
                    chunkSize = textBoxText.Text.Length - pos;
                else
                    chunkSize = xdata.szStringDataA.Length;

                textBoxText.Text.CopyTo(pos, xdata.szStringDataA, 0, chunkSize);
                xdata.Data1 = (UInt32)chunkSize;
                pos += chunkSize;//set new position

                byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);
                SendMessageToServer(byData);
            }

            /**************************************************/
            //Send an EndMessage
            xdata.Data_Type = (UInt16)PACKETTYPES_SUBMESSAGE.SUBMSG_MessageEnd;
            xdata.Data1 = (UInt32)pos;//send the total which should be the 'pos' value
            byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);
            SendMessageToServer(byData);
        }

        private bool ConnectToHostServer()
        {
            try
            {
                pictureBox1.Image = imageListStatusLights.Images["PURPLE"];
                if (client == null)
                {
                    client = new Client();
                    client.OnDisconnected += OnDisconnect;
                    client.OnReceiveData += OnDataReceive;
                }
                else
                {
                    //if we get here then we already have a client object so see if we are already connected
                    if (client.Connected)
                        return true;
                }

                string szIPstr = GetSHubAddress();
                if (szIPstr.Length == 0)
                {
                    pictureBox1.Image = imageListStatusLights.Images["RED"];
                    return false;
                }

                int port = 0;
                if(!Int32.TryParse(textBoxServerListeningPort.Text, out port))
                    port = 9999;

                IPAddress ipAdd = IPAddress.Parse(szIPstr);
                client.Connect(ipAdd, port);//(int)GeneralSettings.HostPort);

                if (client.Connected)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: ConnectToHostServer - {exceptionMessage}");
            }
            return false;
        }

        bool ImDisconnecting = false;
        public void DoServerDisconnect()
        {
            int Line = 0;
            if (ImDisconnecting)
                return;

            ImDisconnecting = true;

            Console.WriteLine("\nIN DoServerDisconnect\n");
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(DoServerDisconnect));
                    return;
                }

                pictureBox1.Image = imageListStatusLights.Images["PURPLE"];

                int i = 0;
                Line = 1;
                

                if (client != null)
                {
                    TellServerImDisconnecting();
                    Thread.Sleep(75);// this is needed!
                }

                Line = 4;

                ServerConnected = false;

                DestroyGeneralTimer();

                Line = 5;
                

                /***************************************************/
                try
                {
                    //bust out of the data loops
                    if (autoEventHostServer != null)
                    {
                        autoEventHostServer.Set();

                        i = 0;
                        while (DataProcessHostServerThread.IsAlive)
                        {
                            Thread.Sleep(1);
                            if (i++ > 200)
                            {
                                DataProcessHostServerThread.Abort();
                                //Debug.WriteLine("\nHAD TO ABORT PACKET THREAD\n");
                                break;
                            }
                        }

                        autoEventHostServer.Close();
                        autoEventHostServer.Dispose();
                        autoEventHostServer = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DoServerDisconnectA: {ex.Message}");
                }

                Line = 8;
                if (autoEvent2 != null)
                {
                    autoEvent2.Set();

                    autoEvent2.Close();
                    autoEvent2.Dispose();
                    autoEvent2 = null;
                }
                /***************************************************/

                Line = 9;
                //Debug.WriteLine("AppIsExiting = " + AppIsExiting.ToString());
                if (client != null)
                {
                    if (client.OnReceiveData != null)
                        client.OnReceiveData -= OnDataReceive;
                    if (client.OnDisconnected != null)
                        client.OnDisconnected -= OnDisconnect;

                    client.Disconnect();
                    client = null;
                }

                Line = 10;

                try
                {
                    Line = 13;
                    //buttonConnect.Text = "Connect";
                    labelStatusInfo.Text = "NOT Connected";
                    Line = 14;
                    labelStatusInfo.ForeColor = System.Drawing.Color.Red;
                }
                catch { }
                Line = 15;

                buttonConnectToServer.Enabled = true;
                pictureBox1.Image = imageListStatusLights.Images["RED"];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DoServerDisconnectB: {ex.Message}");
            }
            finally
            {
                ImDisconnecting = false;
            }

            return;
        }

        private void InitializeServerConnection()
        {
            try
            {
                /**** Packet processor mutex, loop and other support variables *************************/
                autoEventHostServer = new AutoResetEvent(false);//the data mutex
                autoEvent2 = new AutoResetEvent(false);//the FullPacket data mutex
                FullPacketDataProcessThread = new Thread(new ThreadStart(ProcessRecievedServerData));
                DataProcessHostServerThread = new Thread(new ThreadStart(NormalizeServerRawPackets));


                if (HostServerRawPackets == null)
                    HostServerRawPackets = new MotherOfRawPackets(0);
                else
                {
                    HostServerRawPackets.ClearList();
                }

                if (FullHostServerPacketList == null)
                    FullHostServerPacketList = new Queue<FullPacket>();
                else
                {
                    lock (FullHostServerPacketList)
                        FullHostServerPacketList.Clear();
                }
                /***************************************************************************************/
                

                FullPacketDataProcessThread.Start();
                DataProcessHostServerThread.Start();

                labelStatusInfo.Text = "Connecting...";
                labelStatusInfo.ForeColor = System.Drawing.Color.Navy;
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: InitializeServerConnection - {exceptionMessage}");
            }
        }

        #region Callbacks from the TCPIP client layer
        /// <summary>
        /// Data coming in from the TCPIP server
        /// </summary>
        private void OnDataReceive(byte[] message, int messageSize)
        {
            if (AppIsExiting)
                return;
            //Console.WriteLine($"Raw Data From: Host Server, Size of Packet: {messageSize}");
            HostServerRawPackets.AddToList(message, messageSize);
            if (autoEventHostServer != null)
                autoEventHostServer.Set();//Fire in the hole
        }

        /// <summary>
        /// Server disconnected
        /// </summary>
        private void OnDisconnect()
        {
            //Debug.WriteLine("Something Disconnected!! - OnDisconnect()");
            DoServerDisconnect();
        }
        #endregion

        internal void SendMessageToServer(byte[] byData)
        {
            //TimeSpan ts = client.LastDataFromServer

            if (client.Connected)
                client.SendMessage(byData);
        }

        #region Packet factory Processing from server
        private void NormalizeServerRawPackets()
        {
            try
            {
                Console.WriteLine($"NormalizeServerRawPackets ThreadID = {Thread.CurrentThread.ManagedThreadId}");

                while (ServerConnected)
                {
                    //ods.DebugOut("Before AutoEvent");
                    autoEventHostServer.WaitOne(10000);//wait at mutex until signal
                    //autoEventHostServer.WaitOne();//wait at mutex until signal
                    //ods.DebugOut("After AutoEvent");

                    if (AppIsExiting || this.IsDisposed)
                        break;

                    /**********************************************/

                    if (HostServerRawPackets.GetItemCount == 0)
                        continue;

                    //byte[] packetplayground = new byte[45056];//good for 10 full packets(40960) + 1 remainder(4096)
                    byte[] packetplayground = new byte[11264];//good for 10 full packets(10240) + 1 remainder(1024)
                    RawPackets rp;

                    int actualPackets = 0;

                    while (true)
                    {
                        if (HostServerRawPackets.GetItemCount == 0)
                            break;

                        int holdLen = 0;

                        if (HostServerRawPackets.bytesRemaining > 0)
                            Copy(HostServerRawPackets.Remainder, 0, packetplayground, 0, HostServerRawPackets.bytesRemaining);

                        holdLen = HostServerRawPackets.bytesRemaining;

                        for (int i = 0; i < 10; i++)//only go through a max of 10 times so there will be room for any remainder
                        {
                            rp = HostServerRawPackets.GetTopItem;

                            Copy(rp.dataChunk, 0, packetplayground, holdLen, rp.iChunkLen);

                            holdLen += rp.iChunkLen;

                            if (HostServerRawPackets.GetItemCount == 0)//make sure there is more in the list befor continuing
                                break;
                        }

                        actualPackets = 0;

                        #region new PACKET_SIZE 1024
                        if (holdLen >= 1024)//make sure we have at least one packet in there
                        {
                            actualPackets = holdLen / 1024;
                            HostServerRawPackets.bytesRemaining = holdLen - (actualPackets * 1024);

                            for (int i = 0; i < actualPackets; i++)
                            {
                                byte[] tmpByteArr = new byte[1024];
                                Copy(packetplayground, i * 1024, tmpByteArr, 0, 1024);
                                lock (FullHostServerPacketList)
                                    FullHostServerPacketList.Enqueue(new FullPacket(HostServerRawPackets.iListClientID, tmpByteArr));
                            }
                        }
                        else
                        {
                            HostServerRawPackets.bytesRemaining = holdLen;
                        }

                        //hang onto the remainder
                        Copy(packetplayground, actualPackets * 1024, HostServerRawPackets.Remainder, 0, HostServerRawPackets.bytesRemaining);
                        #endregion


                        if (FullHostServerPacketList.Count > 0)
                            autoEvent2.Set();

                    }//end of while(true)
                    /**********************************************/
                }

                Console.WriteLine("Exiting the packet normalizer");
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: NormalizeServerRawPackets - {exceptionMessage}");
            }
        }

        private void ProcessRecievedServerData()
        {
            try
            {
                Console.WriteLine($"ProcessRecievedHostServerData ThreadID = {Thread.CurrentThread.ManagedThreadId}");
                while (ServerConnected)
                {
                    //ods.DebugOut("Before AutoEvent2");
                    autoEvent2.WaitOne(10000);//wait at mutex until signal
                    //autoEvent2.WaitOne();
                    //ods.DebugOut("After AutoEvent2");
                    if (AppIsExiting || !ServerConnected || this.IsDisposed)
                        break;

                    while (FullHostServerPacketList.Count > 0)
                    {
                        try
                        {
                            FullPacket fp;
                            lock (FullHostServerPacketList)
                                fp = FullHostServerPacketList.Dequeue();

                            UInt16 type = (ushort)(fp.ThePacket[1] << 8 | fp.ThePacket[0]);
                            //Debug.WriteLine("Got Server data... Packet type: " + ((PACKETTYPES)type).ToString());
                            switch (type)//Interrogate the first 2 Bytes to see what the packet TYPE is
                            {
                                case (Byte)PACKETTYPES.TYPE_RequestCredentials:
                                    {
                                        ReplyToHostCredentialRequest(fp.ThePacket);
                                        //(new Thread(() => ReplyToHostCredentialRequest(fp.ThePacket))).Start();//
                                    }
                                    break;
                                case (Byte)PACKETTYPES.TYPE_Ping:
                                    {
                                        ReplyToHostPing(fp.ThePacket);
                                        Console.WriteLine($"Received Ping: {GeneralFunction.GetDateTimeFormatted}");
                                    }
                                    break;
                                case (Byte)PACKETTYPES.TYPE_HostExiting:
                                    HostCommunicationsHasQuit(true);
                                    break;
                                case (Byte)PACKETTYPES.TYPE_Registered:
                                    {
                                        SetConnectionsStatus();
                                    }
                                    break;
                                case (Byte)PACKETTYPES.TYPE_MessageReceived:
                                    pictureBox1.Image = imageListStatusLights.Images["GREEN"];
                                    break;
                            }
                            
                            if (client != null)
                                client.LastDataFromServer = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                            Console.WriteLine($"EXCEPTION IN: ProcessRecievedServerData A - {exceptionMessage}");
                        }
                    }//end while
                }//end while serverconnected

                //ods.DebugOut("Exiting the ProcessRecievedHostServerData() thread");
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: ProcessRecievedServerData B - {exceptionMessage}");
            }
        }
        #endregion

        private void SetConnectionsStatus()
        {
            Int32 loc = 1;
            try
            {
                if (InvokeRequired)
                {
                    loc = 5;
                    this.Invoke(new MethodInvoker(SetConnectionsStatus));
                    return;
                }
                loc = 10;
                pictureBox1.Image = imageListStatusLights.Images["GREEN"];
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: SetConnectionsStatus - {exceptionMessage}");
            }
        }

        #region Packets
        private void ReplyToHostPing(byte[] message)
        {
            try
            {
                PACKET_DATA IncomingData = new PACKET_DATA();
                IncomingData = (PACKET_DATA)PACKET_FUNCTIONS.ByteArrayToStructure(message, typeof(PACKET_DATA));

                /****************************************************************************************/
                //calculate how long that ping took to get here
                TimeSpan ts = (new DateTime(IncomingData.DataLong1)) - (new DateTime(ServerTime));
                Console.WriteLine($"{GeneralFunction.GetDateTimeFormatted}: {string.Format("Ping From Server to client: {0:0.##}ms", ts.TotalMilliseconds)}");
                /****************************************************************************************/

                ServerTime = IncomingData.DataLong1;// Server computer's current time!

                PACKET_DATA xdata = new PACKET_DATA();

                xdata.Packet_Type = (UInt16)PACKETTYPES.TYPE_PingResponse;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                xdata.DataLong1 = IncomingData.DataLong1;

                byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

                SendMessageToServer(byData);

                CheckThisComputersTimeAgainstServerTime();
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: ReplyToHostPing - {exceptionMessage}");
            }
        }

        private void CheckThisComputersTimeAgainstServerTime()
        {
            Int64 timeDiff = DateTime.UtcNow.Ticks - ServerTime;
            TimeSpan ts = TimeSpan.FromTicks(Math.Abs(timeDiff));
            Console.WriteLine($"Server diff in secs: {ts.TotalSeconds}");

            if (ts.TotalMinutes > 15)
            {
                string msg = string.Format("Computer Time Discrepancy!! " +
                    "The time on this computer differs greatly " +
                    "compared to the time on the Realtrac Server " +
                    "computer. Check this PC's time.");

                Console.WriteLine(msg);
            }
        }
        
        public void ReplyToHostCredentialRequest(byte[] message)
        {
            if (client == null)
                return;
            
            Console.WriteLine($"ReplyToHostCredentialRequest ThreadID = {Thread.CurrentThread.ManagedThreadId}");
            Int32 Loc = 0;
            try
            {
                //We will assume to tell the host this is just an update of the
                //credentials we first sent during the application start. This
                //will be true if the 'message' argument is null, otherwise we
                //will change the packet type below to the 'TYPE_MyCredentials'.
                UInt16 PaketType = (UInt16)PACKETTYPES.TYPE_CredentialsUpdate;

                if (message != null)
                {
                    int myOldServerID = 0;
                    //The host server has past my ID.
                    PACKET_DATA IncomingData = new PACKET_DATA();
                    IncomingData = (PACKET_DATA)PACKET_FUNCTIONS.ByteArrayToStructure(message, typeof(PACKET_DATA));
                    Loc = 10;
                    if (MyHostServerID > 0)
                        myOldServerID = MyHostServerID;
                    Loc = 20;
                    MyHostServerID = (int)IncomingData.idTo;//Hang onto this value
                    Loc = 25;
                    
                    Console.WriteLine($"My Host Server ID is {MyHostServerID}");

                    string MyAddressAsSeenByTheHost = new string(IncomingData.szStringDataA).TrimEnd('\0');//My computer address
                    SetSomeLabelInfoFromThread($"My Address As Seen By The Server: {MyAddressAsSeenByTheHost}, and my ID given by the server is: {MyHostServerID}");
                    
                    ServerTime = IncomingData.DataLong1;

                    PaketType = (UInt16)PACKETTYPES.TYPE_MyCredentials;
                }

                //ods.DebugOut("Send Host Server some info about myself");
                PACKET_DATA xdata = new PACKET_DATA();

                xdata.Packet_Type = PaketType;
                xdata.Data_Type = 0;
                xdata.Packet_Size = (UInt16)Marshal.SizeOf(typeof(PACKET_DATA));
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                //Station Name
                string p = System.Environment.MachineName;
                if (p.Length > (xdata.szStringDataA.Length - 1))
                    p.CopyTo(0, xdata.szStringDataA, 0, (xdata.szStringDataA.Length - 1));
                else
                    p.CopyTo(0, xdata.szStringDataA, 0, p.Length);
                xdata.szStringDataA[(xdata.szStringDataA.Length - 1)] = '\0';//cap it off just incase

                //App and DLL Version
                string VersionNumber = string.Empty;

                VersionNumber = Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." +
                                    Assembly.GetEntryAssembly().GetName().Version.Minor.ToString() + "." +
                                    Assembly.GetEntryAssembly().GetName().Version.Build.ToString();

                Loc = 30;

                VersionNumber.CopyTo(0, xdata.szStringDataB, 0, VersionNumber.Length);
                Loc = 40;
                //Station Name
                string L = textBoxClientName.Text;
                if (L.Length > (xdata.szStringData150.Length - 1))
                    L.CopyTo(0, xdata.szStringData150, 0, (xdata.szStringData150.Length - 1));
                else
                    L.CopyTo(0, xdata.szStringData150, 0, L.Length);
                xdata.szStringData150[(xdata.szStringData150.Length - 1)] = '\0';//cap it off just incase

                Loc = 50;

                //Application type
                xdata.nAppLevel = (UInt16)APPLEVEL.None;
                
                
                byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);
                Loc = 60;
                SendMessageToServer(byData);
                Loc = 70;
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION at location {Loc}, IN: ReplyToHostCredentialRequest - {exceptionMessage}");
            }
        }

        private delegate void SetSomeLabelInfoDelegate(string info);
        private void SetSomeLabelInfoFromThread(string info)
        {
            if(InvokeRequired)
            {
                this.Invoke(new SetSomeLabelInfoDelegate(SetSomeLabelInfoFromThread), new object[] { info });
                return;
            }

            labelConnectionStuff.Text = info;
        }

        private delegate void HostCommunicationsHasQuitDelegate(bool FromHost);
        private void HostCommunicationsHasQuit(bool FromHost)
        {
            if (InvokeRequired)
            {
                this.Invoke(new HostCommunicationsHasQuitDelegate(HostCommunicationsHasQuit), new object[] { FromHost });
                return;
            }

            if (client != null)
            {
                int c = 100;
                do
                {
                    c--;
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
                while (c > 0);

                DoServerDisconnect();
                
                if (FromHost)
                {
                    labelStatusInfo.Text = "The Server has exited";
                }
                else
                {
                    labelStatusInfo.Text = "App has lost communication with the server (network issue).";
                }

                labelStatusInfo.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void TellServerImDisconnecting()
        {
            try
            {
                PACKET_DATA xdata = new PACKET_DATA();

                xdata.Packet_Type = (UInt16)PACKETTYPES.TYPE_Close;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

                SendMessageToServer(byData);
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: TellServerImDisconnecting - {exceptionMessage}");
            }
        }
        #endregion

        #region General Timer
        /// <summary>
        /// This will watch the TCPIP communication, after 5 minutes of no communications with the 
        /// Server we will assume the connections has been severed
        /// </summary>
        private void BeginGeneralTimer()
        {
            //create the general timer but skip over it if its already running
            if (GeneralTimer == null)
            {
                GeneralTimer = new System.Windows.Forms.Timer();
                GeneralTimer.Tick += new EventHandler(GeneralTimer_Tick);
                GeneralTimer.Interval = 5000;
                GeneralTimer.Enabled = true;
            }
        }
        
        private void GeneralTimer_Tick(object sender, EventArgs e)
        {
            if (client != null)
            {
                TimeSpan ts = DateTime.Now - client.LastDataFromServer;

                //If we dont hear from the server for more than 5 minutes then there is a problem so disconnect
                if (ts.TotalMinutes > 5)
                {
                    DestroyGeneralTimer();
                    HostCommunicationsHasQuit(false);
                }
            }
            
            // Add 5 seconds worth of Ticks to the server time
            ServerTime += (TimeSpan.TicksPerSecond * 5);
            //Console.WriteLine("SERVER TIME: " + (new DateTime(GeneralFunction.ServerTime)).ToLocalTime().TimeOfDay.ToString());
        }

        private void DestroyGeneralTimer()
        {
            if (GeneralTimer != null)
            {
                if (GeneralTimer.Enabled == true)
                    GeneralTimer.Enabled = false;

                try
                {
                    GeneralTimer.Tick -= GeneralTimer_Tick;
                }
                catch (Exception)
                {
                    //just incase there was no event to remove
                }
                GeneralTimer.Dispose();
                GeneralTimer = null;
            }
        }
        #endregion//General Timer section

        private string GetSHubAddress()//translates a named IP to an address
        {
            string SHubServer = textBoxServer.Text; //GeneralSettings.HostIP.Trim();

            if (SHubServer.Length < 1)
                return string.Empty;

            try
            {
                string[] qaudNums = SHubServer.Split('.');

                // See if its not a straightup IP address.. 
                //if not then we have to resolve the computer name
                if (qaudNums.Length != 4)
                {
                    //Must be a name so see if we can resolve it
                    IPHostEntry hostEntry = Dns.GetHostEntry(SHubServer);

                    foreach (IPAddress a in hostEntry.AddressList)
                    {
                        if (a.AddressFamily == AddressFamily.InterNetwork)//use IP 4 for now
                        {
                            SHubServer = a.ToString();
                            break;
                        }
                    }
                    //SHubServer = hostEntry.AddressList[0].ToString();
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Exception: {se.Message}");
                //statusStrip1.Items[1].Text = se.Message + " for " + Properties.Settings.Default.HostIP;
                SHubServer = string.Empty;
            }

            return SHubServer;
        }

        private void CheckOnApplicationDirectory()
        {
            try
            {
                string AppPath = GeneralFunction.GetAppPath;

                if (!Directory.Exists(AppPath))
                {
                    Directory.CreateDirectory(AppPath);
                }
            }
            catch
            {
                Console.WriteLine("ISSUE CREATING A DIRECTORY");
            }
        }

        #region UNSAFE CODE
        // The unsafe keyword allows pointers to be used within the following method:
        static unsafe void Copy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            try
            {
                if (src == null || srcIndex < 0 || dst == null || dstIndex < 0 || count < 0)
                {
                    Console.WriteLine("Serious Error in the Copy function 1");
                    throw new System.ArgumentException();
                }

                int srcLen = src.Length;
                int dstLen = dst.Length;
                if (srcLen - srcIndex < count || dstLen - dstIndex < count)
                {
                    Console.WriteLine("Serious Error in the Copy function 2");
                    throw new System.ArgumentException();
                }

                // The following fixed statement pins the location of the src and dst objects
                // in memory so that they will not be moved by garbage collection.
                fixed (byte* pSrc = src, pDst = dst)
                {
                    byte* ps = pSrc + srcIndex;
                    byte* pd = pDst + dstIndex;

                    // Loop over the count in blocks of 4 bytes, copying an integer (4 bytes) at a time:
                    for (int i = 0; i < count / 4; i++)
                    {
                        *((int*)pd) = *((int*)ps);
                        pd += 4;
                        ps += 4;
                    }

                    // Complete the copy by moving any bytes that weren't moved in blocks of 4:
                    for (int i = 0; i < count % 4; i++)
                    {
                        *pd = *ps;
                        pd++;
                        ps++;
                    }
                }
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine("EXCEPTION IN: Copy - " + exceptionMessage);
            }

        }

        #endregion
    }
}
