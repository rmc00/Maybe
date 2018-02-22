using System.IO;

namespace Maybe.Utilities
{
    /// <summary>
    /// Helper class to abstract serializing objects to bytes.
    /// </summary>
    public static class ByteConverter
    {
        /// <summary>
        /// Given a serializable object, returns the binary serialized representation of that object.
        /// </summary>
        /// <param name="item">The input to be serialized</param>
        /// <returns>Binary serialized representation of the input item.</returns>
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
