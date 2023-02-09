using System;
using System.IO;
using MessagePack;
using MessagePack.LZ4;
using MessagePack.Unity.Extension;



public class LZ4 {
    public static ArraySegment<byte> Compress(ArraySegment<byte> serializedData) {
        if (serializedData.Count < LZ4MessagePackSerializer.NotCompressionSize)
        {
            return serializedData;
        }
        else
        {
            var offset = 0;
            var buffer = MessagePackBufferManager.GetLZ4Buffer();
            var maxOutCount = LZ4Codec.MaximumOutputLength(serializedData.Count);
            if (buffer.Length + 6 + 5 < maxOutCount) // (ext header size + fixed length size)
            {
                buffer = new byte[6 + 5 + maxOutCount];
            }

            // acquire ext header position
            var extHeaderOffset = offset;
            offset += (6 + 5);

            // write body
#if UNITY_64
            var lz4Length = LZ4Codec.Encode64Unsafe(serializedData.Array, serializedData.Offset, serializedData.Count, buffer, offset, buffer.Length - offset);
#else
            var lz4Length = LZ4Codec.Encode32Unsafe(serializedData.Array, serializedData.Offset, serializedData.Count, buffer, offset, buffer.Length - offset);
#endif

            // write extension header(always 6 bytes)
            extHeaderOffset += MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref buffer, extHeaderOffset, LZ4MessagePackSerializer.ExtensionTypeCode, lz4Length);

            // write length(always 5 bytes)
            MessagePackBinary.WriteInt32ForceInt32Block(ref buffer, extHeaderOffset, serializedData.Count);

            return new ArraySegment<byte>(buffer, 0, 6 + 5 + lz4Length);
        }
    }
    
    public static byte[] Compress(byte[] bytes) {
        var segment = Compress(new ArraySegment<byte>(bytes, 0, bytes.Length));
        return MessagePackBinary.FastCloneWithResize(segment.Array,segment.Count);
    }
    
    public static byte[] Decompress(byte[] bytes) {
        var resultBuffer = MessagePackBufferManager.GetLZ4Buffer();
        var length = Decompress(new ArraySegment<byte>(bytes, 0, bytes.Length), ref resultBuffer);
        return MessagePackBinary.FastCloneWithResize(resultBuffer, length);
    }
    
    public static int Decompress(byte[] bytes, ref byte[] resultBuffer) {
        return Decompress(new ArraySegment<byte>(bytes, 0, bytes.Length), ref resultBuffer);
    }
    
    public static int Decompress(Stream stream, ref byte[] lz4Buffer, ref byte[] resultBuffer) {
        var length = BinaryHelper.FillFromStream(stream, ref lz4Buffer);
        return Decompress(new ArraySegment<byte>(lz4Buffer, 0, length), ref resultBuffer);
    }
    
    public static int Decompress(ArraySegment<byte> bytes, ref byte[] buffer) {
        if (MessagePackBinary.GetMessagePackType(bytes.Array, bytes.Offset) == MessagePackType.Extension)
        {
            var header = MessagePackBinary.ReadExtensionFormatHeader(bytes.Array, bytes.Offset, out var readSize);
            if (header.TypeCode == LZ4MessagePackSerializer.ExtensionTypeCode)
            {
                // decode lz4
                var offset = bytes.Offset + readSize;
                var outputLength = MessagePackBinary.ReadInt32(bytes.Array, offset, out readSize);
                offset += readSize;

                MessagePackBinary.EnsureCapacity(ref buffer, 0, outputLength);

                // LZ4 Decode
                var inputLength = (int)header.Length;
#if UNITY_64
                return LZ4Codec.Decode64Unsafe(bytes.Array, offset, inputLength, buffer, 0, outputLength);
#else
                return LZ4Codec.Decode32Unsafe(bytes.Array, offset, inputLength, buffer, 0, outputLength);
#endif
            }
        }
        
        MessagePackBinary.EnsureCapacity(ref buffer, 0, bytes.Count);
        Unsafe.MemCpy(bytes.Array, bytes.Offset, buffer, 0, (uint)bytes.Count);
        return bytes.Count;
    }
}