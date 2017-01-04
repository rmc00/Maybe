using Maybe.BloomFilter;
using Xunit;

namespace Maybe.Tests.BloomFilter
{
    public class ScalableBloomFilterTests
    {
        [Fact]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = new ScalableBloomFilter<int>(0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = new ScalableBloomFilter<int>(0.02);
            Assert.False(filter.Contains(42));
        }

        [Fact]
        public void NumberFilters_WithThreeTimesFirstCapacity_ShouldBeTwo()
        {
            var filter = new ScalableBloomFilter<int>(0.02);
            for (var i = 0; i < 3*ScalableBloomFilter<int>.MinimumCapacity; i++)
            {
                filter.Add(i);
            }
            Assert.Equal(2, filter.NumberFilters);
        }
    }
}
