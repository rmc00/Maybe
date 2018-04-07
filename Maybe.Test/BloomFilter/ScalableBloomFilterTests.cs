using Maybe.BloomFilter;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace Maybe.Test.BloomFilter
{
    public class ScalableBloomFilterTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = new ScalableBloomFilter<int>(0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = new ScalableBloomFilter<int>(0.02);
            Assert.False(filter.Contains(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void NumberFilters_WithThreeTimesFirstCapacity_ShouldBeTwo()
        {
            var filter = new ScalableBloomFilter<int>(0.02);
            for (var i = 0; i < 3*ScalableBloomFilter<int>.MinimumCapacity; i++)
            {
                filter.Add(i);
            }
            Assert.Equal(2, filter.NumberFilters);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WhenItemHasBeenAdded_AndFilterHasBeenSerializedAndUnserialized_ShouldReturnTrue()
        {
            using (var stream = new MemoryStream())
            {
                var filterOld = new ScalableBloomFilter<int>(0.02);
                filterOld.Add(42);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, filterOld);
                stream.Flush();
                stream.Position = 0;
                ScalableBloomFilter<int> filterNew = (ScalableBloomFilter<int>)formatter.Deserialize(stream);
                Assert.True(filterNew.Contains(42));
            }
        }
    }
}
