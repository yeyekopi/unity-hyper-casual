using System.Runtime.CompilerServices;

namespace MessagePack.Formatters {
      public sealed class UnsafeArrayFormatter<T> : IMessagePackFormatter<T[]>
     {
        public int Serialize(ref byte[] bytes, int offset, T[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var formatter = formatterResolver.GetFormatterWithVerify<T>();
                return Write(ref bytes, offset, value, formatterResolver, formatter);
            }
        }

        public T[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var formatter = formatterResolver.GetFormatterWithVerify<T>();
                return Read(bytes, offset, formatterResolver, formatter, out readSize);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Write(ref byte[] bytes, int offset, T[] value, IFormatterResolver resolver, IMessagePackFormatter<T> formatter) {
            var startOffset = offset;
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                offset += formatter.Serialize(ref bytes, offset, value[i], resolver);
            }
            return offset - startOffset;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Read(byte[] bytes, int offset, IFormatterResolver resolver, IMessagePackFormatter<T> formatter, out int readSize) {
            var startOffset = offset;
            var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;
            var array = new T[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = formatter.Deserialize(bytes, offset, resolver, out readSize);
                offset += readSize;
            }
            readSize = offset - startOffset;
            return array;
        }
    }
      
      
      public sealed class UnsafeStaticArrayFormatter<T> : IMessagePackFormatter<T[]>
     {
          public UnsafeStaticArrayFormatter(IMessagePackFormatter<T> underlyingFormatter) {
              this.underlyingFormatter = underlyingFormatter;
          }


        readonly IMessagePackFormatter<T> underlyingFormatter; 
        public int Serialize(ref byte[] bytes, int offset, T[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return UnsafeArrayFormatter<T>.Write(ref bytes, offset, value, formatterResolver, underlyingFormatter);
            }
        }

        public T[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return UnsafeArrayFormatter<T>.Read(bytes, offset, formatterResolver, underlyingFormatter, out readSize);
            }
        }
    }
      
}