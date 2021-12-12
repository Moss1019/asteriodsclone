using System.Runtime.Serialization.Formatters.Binary;

namespace WebSockServer
{
    internal static class ByteExtensions
    {
        public static T Deserialize<T> (this byte[] data)
        {
            var deserializer = new BinaryFormatter();
            using var memStream = new MemoryStream(data);
            var result = deserializer.Deserialize(memStream);
            return (T)result;
        }

        public static byte[] Serialize<T>(this T obj)
        {
            var serializer = new BinaryFormatter();
            using var memStream = new MemoryStream();
            serializer.Serialize(memStream, obj);
            return memStream.GetBuffer();
        }
    }
}
