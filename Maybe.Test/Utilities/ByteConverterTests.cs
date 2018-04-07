using System;
using System.Runtime.Serialization;
using Maybe.Utilities;
using Xunit;

namespace Maybe.Test.Utilities
{
    public class ByteConverterTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToByteArray_WithNull_ShouldReturnNull()
        {
            var bytes = ByteConverter.ConvertToByteArray(null);
            Assert.Null(bytes);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToByteArray_WithNonSerializableInput_ShouldThrowException()
        {
            var test = new DontSerializeMe();
            Assert.Throws<SerializationException>(() => ByteConverter.ConvertToByteArray(test));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToByteArray_WithSerializableInput_ShouldReturnBytes()
        {
            var test = new SerializeMe();
            var bytes = ByteConverter.ConvertToByteArray(test);
            Assert.NotNull(bytes);
        }

        private class DontSerializeMe
        {
            public string Test;
            public int Data;
        }

        [Serializable]
        private class SerializeMe
        {
            public string MoreTest;
            public string MoreData;
        }
    }
}
