using System.IO;

namespace Maybe.Utilities
{
    public static class ByteConverter
    {
        public static byte[] ConvertToByteArray(object item)
        {
            if (item == null)
            {
                return null;
            }

            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                return stream.ToArray();
            }
        }
    }
}
