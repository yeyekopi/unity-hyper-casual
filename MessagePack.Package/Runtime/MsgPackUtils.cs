using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MessagePack;
using UnityEngine;
using Unsafe = MessagePack.Unity.Extension.Unsafe;
using MPB = MessagePack.MessagePackBinary;
using static MsgPackUtils;

public static class MsgPackUtils {
    
    public static void WriteNil(ref byte[] bytes, ref int offset) {
        offset += MPB.WriteNil(ref bytes, offset);
    }
    public static bool IsNil(byte[] bytes, int offset) {
        return MPB.IsNil(bytes,offset);
    }
    public static void WriteArrayHeader(ref byte[] bytes, ref int offset, int count) {
        offset += MPB.WriteArrayHeader(ref bytes, offset, count);
    }
    public static void WriteArrayHeaderForceArray32Block (ref byte[] bytes, ref int offset, uint count) {
        offset += MPB.WriteArrayHeaderForceArray32Block(ref bytes, offset, count);
    }
    public static void WriteBoolean(ref byte[] bytes, ref int offset, bool value) {
        offset += MPB.WriteBoolean(ref bytes, offset, value);
    }
    public static void WriteByte(ref byte[] bytes, ref int offset, byte value) {
        offset += MPB.WriteByte(ref bytes, offset, value);
    }
    public static void WriteByteForceByteBlock(ref byte[] bytes, ref int offset, byte value) {
        offset += MPB.WriteByteForceByteBlock(ref bytes, offset, value);
    }
    public static void WriteBytes(ref byte[] bytes, ref int offset, byte[] value) {
        offset += MPB.WriteBytes(ref bytes, offset, value);
    }
    public static void WriteSByte(ref byte[] bytes, ref int offset, sbyte value) {
        offset += MPB.WriteSByte(ref bytes, offset, value);
    }
    public static void WriteSByteForceSByteBlock(ref byte[] bytes, ref int offset, sbyte value) {
        offset += MPB.WriteSByteForceSByteBlock(ref bytes, offset, value);
    }
    public static void WriteSingle(ref byte[] bytes, ref int offset, float value) {
        offset += MPB.WriteSingle(ref bytes, offset, value);
    }
    public static void WriteDouble(ref byte[] bytes, ref int offset, double value) {
        offset += MPB.WriteDouble(ref bytes, offset, value);
    }
    public static void WriteInt16(ref byte[] bytes, ref int offset, short value) {
        offset += MPB.WriteInt16(ref bytes, offset, value);
    }
    public static void WriteInt16ForceInt16Block(ref byte[] bytes, ref int offset, short value) {
        offset += MPB.WriteInt16ForceInt16Block(ref bytes, offset, value);
    }
    public static void WritePositiveFixedIntUnsafe(ref byte[] bytes, ref int offset, int value) {
        offset += MPB.WritePositiveFixedIntUnsafe(ref bytes, offset, value);
    }
    public static void WriteInt32(ref byte[] bytes, ref int offset, int value) {
        offset += MPB.WriteInt32(ref bytes, offset, value);
    }
    public static void WriteInt32ForceInt32Block(ref byte[] bytes, ref int offset, int value) {
        offset += MPB.WriteInt32ForceInt32Block(ref bytes, offset, value);
    }
    public static void WriteInt64(ref byte[] bytes, ref int offset, long value) {
        offset += MPB.WriteInt64(ref bytes, offset, value);
    }
    public static void WriteInt64ForceInt64Block(ref byte[] bytes, ref int offset, long value) {
        offset += MPB.WriteInt64ForceInt64Block(ref bytes, offset, value);
    }
    public static void WriteUInt16(ref byte[] bytes, ref int offset, ushort value) {
        offset += MPB.WriteUInt16(ref bytes, offset, value);
    }
    public static void WriteUInt16ForceUInt16Block(ref byte[] bytes, ref int offset, ushort value) {
        offset += MPB.WriteUInt16ForceUInt16Block(ref bytes, offset, value);
    }
    public static void WriteUInt32(ref byte[] bytes, ref int offset, uint value) {
        offset += MPB.WriteUInt32(ref bytes, offset, value);
    }
    public static void WriteUInt32ForceUInt32Block(ref byte[] bytes, ref int offset, uint value) {
        offset += MPB.WriteUInt32ForceUInt32Block(ref bytes, offset, value);
    }
    public static void WriteUInt64(ref byte[] bytes, ref int offset, ulong value) {
        offset += MPB.WriteUInt64(ref bytes, offset, value);
    }
    public static void WriteUInt64ForceUInt64Block(ref byte[] bytes, ref int offset, ulong value) {
        offset += MPB.WriteUInt64ForceUInt64Block(ref bytes, offset, value);
    }
    public static void WriteChar(ref byte[] bytes, ref int offset, char value) {
        offset += MPB.WriteChar(ref bytes, offset, value);
    }
    public static void WriteFixedStringUnsafe(ref byte[] bytes, ref int offset, string value, int byteCount) {
        offset += MPB.WriteFixedStringUnsafe(ref bytes, offset, value,byteCount);
    }
    public static void WriteStringUnsafe(ref byte[] bytes, ref int offset, string value, int byteCount) {
        offset += MPB.WriteStringUnsafe(ref bytes, offset, value,byteCount);
    }
    public static void WriteStringBytes(ref byte[] bytes, ref int offset, byte[] utf8stringBytes) {
        offset += MPB.WriteStringBytes(ref bytes, offset, utf8stringBytes);
    }
    public static void WriteString(ref byte[] bytes, ref int offset, string value) {
        offset += MPB.WriteString(ref bytes, offset, value);
    }
    public static void WriteStringForceStr32Block(ref byte[] bytes, ref int offset, string value) {
        offset += MPB.WriteStringForceStr32Block(ref bytes, offset, value);
    }
    
    public static Nil ReadNil(byte[] bytes, ref int offset){
        var result = MPB.ReadNil(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    }
    public static int ReadArrayHeader(byte[] bytes, ref int offset) {
        var result = MPB.ReadArrayHeader(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static uint ReadArrayHeaderRaw(byte[] bytes, ref int offset) {
        var result = MPB.ReadArrayHeaderRaw(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static bool ReadBoolean(byte[] bytes, ref int offset) {
        var result = MPB.ReadBoolean(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static byte ReadByte(byte[] bytes, ref int offset) {
        var result = MPB.ReadByte(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static byte[] ReadBytes(byte[] bytes, ref int offset) {
        var result = MPB.ReadBytes(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static sbyte ReadSByte(byte[] bytes, ref int offset) {
        var result = MPB.ReadSByte(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static float ReadSingle(byte[] bytes, ref int offset) {
        var result = MPB.ReadSingle(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static double ReadDouble(byte[] bytes, ref int offset) {
        var result = MPB.ReadDouble(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static short ReadInt16(byte[] bytes, ref int offset) {
        var result = MPB.ReadInt16(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static int ReadInt32(byte[] bytes, ref int offset) {
        var result = MPB.ReadInt32(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static long ReadInt64(byte[] bytes, ref int offset) {
        var result = MPB.ReadInt64(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static ushort ReadUInt16(byte[] bytes, ref int offset) {
        var result = MPB.ReadUInt16(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static uint ReadUInt32(byte[] bytes, ref int offset) {
        var result = MPB.ReadUInt32(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static ulong ReadUInt64(byte[] bytes, ref int offset) {
        var result = MPB.ReadUInt64(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static byte[] ReadBytesSafe(byte[] bytes, int length, ref int offset) { 
        var result = MPB.ReadBytesSafe(bytes, offset, length, out var readSize);
        offset += readSize;
        return result;
    } 
    public static char ReadChar(byte[] bytes, ref int offset) {
        var result = MPB.ReadChar(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    } 
    public static string ReadString(byte[] bytes, ref int offset) {
        var result = MPB.ReadString(bytes, offset, out var readSize);
        offset += readSize;
        return result;
    }

    public static int ReadNextBlock(byte[] bytes, ref int offset) {
        var readSize = MPB.ReadNextBlock(bytes, offset);
        offset += readSize;
        return readSize;
    }

}

public static class BinaryHelper {
    // no messagepack typecodes. MessagePackBinary.ReadByte wont work
    public static int WriteUInt8(ref byte[] bytes, int offset, byte value) {
        MPB.EnsureCapacity(ref bytes, offset, 1);
        bytes[offset] = value;
        return 1;
    }

    public static byte ReadUInt8(byte[] bytes, int offset, out int readSize) {
        readSize = 1;
        return bytes[offset];
    }
    
    // no messagepack typecodes. MessagePackBinary.ReadSByte wont work
    public static int WriteInt8(ref byte[] bytes, int offset, sbyte value) {
        MPB.EnsureCapacity(ref bytes, offset, 1);
        bytes[offset] = unchecked((byte)value);
        return 1;
    }

    public static sbyte ReadInt8(byte[] bytes, int offset, out int readSize) {
        readSize = 1;
        return unchecked((sbyte)bytes[offset]);
    }

    // no messagepack typecodes. MessagePackBinary.ReadUInt16 wont work
    public static int WriteUInt16(ref byte[] bytes, int offset, ushort value) {
        MPB.EnsureCapacity(ref bytes, offset, 2);
        bytes[offset] = unchecked((byte)(value >> 8));
        bytes[offset + 1] = unchecked((byte)value);
        return 2;
    }
    
    public static ushort ReadUInt16(byte[] bytes, int offset, out int readSize) {
        readSize = 2;
        unchecked {
            return (ushort)((bytes[offset] << 8) | (bytes[offset + 1]));
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteSingleArray(ref byte[] bytes, float[] value, int offset) {
        var byteCount =  value.Length * 4;
        MessagePackBinary.EnsureCapacity(ref bytes, offset, byteCount);
        Unsafe.MemCpy(value,0, bytes, offset, (uint)value.Length * 4);
        // Buffer.BlockCopy(value,0, bytes, offset, value.Length * 4);
        return byteCount;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ReadSingleArray(byte[] bytes, int offset, out int readSize, int count) {
        var data = new float[count];
        var byteCount = count * 4;
        Unsafe.MemCpy(bytes, offset, data, 0, (uint)byteCount);
        // Buffer.BlockCopy(bytes, offset, data, 0, byteCount);
        readSize = byteCount;
        return data;
    }
    
    public static int FillFromStream(Stream stream, ref byte[] buffer) {
        var length = 0;
        int readSize;
        while ((readSize = stream.Read(buffer, length, buffer.Length - length)) > 0) {
            length += readSize;
            if (length == buffer.Length) {
                MessagePackBinary.FastResize(ref buffer, buffer.Length * 2);
            }
        }
        return length;
    }
}