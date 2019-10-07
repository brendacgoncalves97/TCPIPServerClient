using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;


[Flags]
public enum APPLICATION
{
    None = 0,
    ClientTypeA = 1,
    ClientTypeB = 2
}

public enum APPLEVEL
{
    None = 0,
    ClientLevel1 = 1,
    ClientLevel2 = 2
}

public enum PACKETTYPES
{
    TYPE_Ping = 1,
    TYPE_PingResponse = 2,
    TYPE_RequestCredentials = 3,
    TYPE_MyCredentials = 4,
    TYPE_Registered = 5,
    TYPE_HostExiting = 6,
    TYPE_ClientData = 7,
    TYPE_ClientDisconnecting = 8,
    TYPE_CredentialsUpdate = 9,
    TYPE_Close = 10,
    TYPE_Message = 11,
    TYPE_MessageReceived = 12
}

public enum PACKETTYPES_SUBMESSAGE
{
    SUBMSG_MessageStart,
    SUBMSG_MessageGuts,
    SUBMSG_MessageEnd
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PACKET_DATA
{
    /****************************************************************/
    //HEADER is 18 BYTES
    public UInt16 Packet_Type;  //TYPE_??
    public UInt16 Packet_Size;
    public UInt16 Data_Type;	// DATA_ type fields
    public UInt16 maskTo;		// SENDTO_MY_SHUBONLY and the like.
    public UInt32 idTo;			// Used if maskTo is SENDTO_INDIVIDUAL
    public UInt32 idFrom;		// Client ID value
    public UInt16 nAppLevel;
    /****************************************************************/

    public UInt32 Data1;        //miscellanious information
    public UInt32 Data2;        //miscellanious information
    public UInt32 Data3;        //miscellanious information
    public UInt32 Data4;        //miscellanious information
    public UInt32 Data5;        //miscellanious information

    public Int32 Data6;        //miscellanious information
    public Int32 Data7;        //miscellanious information
    public Int32 Data8;        //miscellanious information
    public Int32 Data9;        //miscellanious information
    public Int32 Data10;       //miscellanious information

    public UInt32 Data11;        //miscellanious information
    public UInt32 Data12;        //miscellanious information
    public UInt32 Data13;        //miscellanious information
    public UInt32 Data14;        //miscellanious information
    public UInt32 Data15;        //miscellanious information

    public Int32 Data16;        //miscellanious information
    public Int32 Data17;        //miscellanious information
    public Int32 Data18;        //miscellanious information
    public Int32 Data19;        //miscellanious information
    public Int32 Data20;       //miscellanious information

    public UInt32 Data21;        //miscellanious information
    public UInt32 Data22;        //miscellanious information
    public UInt32 Data23;        //miscellanious information
    public UInt32 Data24;        //miscellanious information
    public UInt32 Data25;        //miscellanious information

    public Int32 Data26;        //miscellanious information
    public Int32 Data27;        //miscellanious information
    public Int32 Data28;        //miscellanious information
    public Int32 Data29;        //miscellanious information
    public Int32 Data30;       //miscellanious information

    public Double DataDouble1;
    public Double DataDouble2;
    public Double DataDouble3;
    public Double DataDouble4;
    public Double DataDouble5;

    /// <summary>
    /// Long value1
    /// </summary>
    public Int64 DataLong1;
    /// <summary>
    /// Long value2
    /// </summary>
    public Int64 DataLong2;
    /// <summary>
    /// Long value3
    /// </summary>
    public Int64 DataLong3;
    /// <summary>
    /// Long value4
    /// </summary>
    public Int64 DataLong4;

    /// <summary>
    /// Unsigned Long value1
    /// </summary>
    public UInt64 DataULong1;
    /// <summary>
    /// Unsigned Long value2
    /// </summary>
    public UInt64 DataULong2;
    /// <summary>
    /// Unsigned Long value3
    /// </summary>
    public UInt64 DataULong3;
    /// <summary>
    /// Unsigned Long value4
    /// </summary>
    public UInt64 DataULong4;

    /// <summary>
    /// DateTime Tick value1
    /// </summary>
    public Int64 DataTimeTick1;

    /// <summary>
    /// DateTime Tick value2
    /// </summary>
    public Int64 DataTimeTick2;

    /// <summary>
    /// DateTime Tick value1
    /// </summary>
    public Int64 DataTimeTick3;

    /// <summary>
    /// DateTime Tick value2
    /// </summary>
    public Int64 DataTimeTick4;

    /// <summary>
    /// 300 Chars
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 300)]
    public Char[] szStringDataA = new Char[300];

    /// <summary>
    /// 300 Chars
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 300)]
    public Char[] szStringDataB = new Char[300];

    /// <summary>
    /// 150 Chars
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
    public Char[] szStringData150 = new Char[150];

    //18 + 120 + 40 + 96 + 600 + 150 = 1024
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PACKET_BIGSTRING
{
    public UInt16 Packet_Type;  //TYPE_??
    public UInt32 idTo;
    public UInt32 idFrom;
    public UInt32 StringLength;
    public UInt32 Extra;

    //1024 - 18  = 1006 bytes are left
    //4096 - 18  = 4078 bytes are left
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1006)]
    public Char[] sBigString = new Char[1006];

    /* NOTE: 
     * Remember to do a search for the value '1006' in 
     * the project if you change the packet size here 
     * and replace the value to the new size
    */
}


public static class PACKET_FUNCTIONS
{
    #region Byte Combobulator and Discombobulator
    public static Byte[] StructureToByteArray(Object obj)
    {
        Int32 rawsize = Marshal.SizeOf(obj);
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.StructureToPtr(obj, buffer, false);
        Byte[] rawdatas = new Byte[rawsize];
        Marshal.Copy(buffer, rawdatas, 0, rawsize);
        Marshal.FreeHGlobal(buffer);
        return rawdatas;
    }

    public static Object ByteArrayToStructure(Byte [] rawdatas, Type anytype)
    {
        Int32 rawsize = Marshal.SizeOf(anytype);
        if (rawsize > rawdatas.Length)
            return null;

        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.Copy(rawdatas, 0, buffer, rawsize);
        Object retobj = Marshal.PtrToStructure(buffer, anytype);
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }
    #endregion
}

public static class EnumerationExtensions
{
    // From:
    // http://stackoverflow.com/questions/93744/most-common-c-sharp-bitwise-operations-on-enums
    // version 2 if this doesn't work properly: http://hugoware.net/blog/enumeration-extensions-2-0
    /*
        SomeType value = SomeType.Grapes;
        bool isGrapes = value.Is(SomeType.Grapes); //true
        bool hasGrapes = value.Has(SomeType.Grapes); //true

        value = value.Add(SomeType.Oranges);
        value = value.Add(SomeType.Apples);
        value = value.Remove(SomeType.Grapes);

        bool hasOranges = value.Has(SomeType.Oranges); //true
        bool isApples = value.Is(SomeType.Apples); //false
        bool hasGrapes = value.Has(SomeType.Grapes); //false

    */
    public static bool Has<T>(this System.Enum type, T value)
    {
        try
        {
            return (((int)(object)type & (int)(object)value) == (int)(object)value);
        }
        catch
        {
            return false;
        }
    }

    public static bool Is<T>(this System.Enum type, T value)
    {
        try
        {
            return (int)(object)type == (int)(object)value;
        }
        catch
        {
            return false;
        }
    }


    public static T Add<T>(this System.Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type | (int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                string.Format(
                    "Could not append value from enumerated type '{0}'.",
                    typeof(T).Name
                    ), ex);
        }
    }

    public static T Remove<T>(this System.Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type & ~(int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                string.Format(
                    "Could not remove value from enumerated type '{0}'.",
                    typeof(T).Name
                    ), ex);
        }
    }

}

