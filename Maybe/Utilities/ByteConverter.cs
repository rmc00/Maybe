using System.Text;
using Newtonsoft.Json;

namespace Maybe.Utilities
{
    public static class ByteConverter
    {
        public static byte[] ConvertToByteArray(object item) => item == null ? null : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
    }
}
