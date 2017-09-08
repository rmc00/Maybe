using System.IO;
using System.Text;

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
            else
            {
#if NETSTANDARD1_0
                /*
                 * NETSTANDARD1_* didn't implement BinaryFromatter class
                 * Use DataContractJsonSerializer instead for serialization
                 */
                var formatter = new System.Runtime.Serialization.Json.DataContractJsonSerializer(item.GetType());
#else
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
#endif
                using (var stream = new MemoryStream())
                {
#if NETSTANDARD1_0
                    formatter.WriteObject(stream, item);
#else
                    formatter.Serialize(stream, item);
#endif
                    return stream.ToArray();
                }
            }
        }
    }
}
