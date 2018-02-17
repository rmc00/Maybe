using System.Linq;
using Maybe.BloomFilter;
using Xunit;

namespace Maybe.Test.BloomFilter
{
    public class BloomFilterTests
    {
        [Fact]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = BloomFilter<int>.Create(50, 0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = BloomFilter<int>.Create(50, 0.02);
            Assert.False(filter.Contains(42));
        }

        [Theory]
        [InlineData(100, 0.05d)]
        [InlineData(1000, 0.05d)]
        [InlineData(10000, 0.05d)]
        public void Contains_With5PercentFalsePositives_ShouldHaveLessThan5PercentErrors(int stepRange, double errorRate)
        {
            var filter = BloomFilter<int>.Create(stepRange, errorRate);
            foreach (var num in Enumerable.Range(1, stepRange))
            {
                filter.Add(num);
            }

            var errorCount = Enumerable.Range(stepRange + 1, stepRange * 2).Count(num => filter.Contains(num));

            Assert.InRange(errorCount, 0d, errorRate * stepRange);
        }

        [Fact]
        public void FillRatio_WithNewFilter_ShouldBeZero()
        {
            var filter = BloomFilter<int>.Create(1000, 0.05);
            Assert.Equal(0d, filter.FillRatio);
        }

        [Fact]
        public void FillRatio_WithOneItem_ShouldBeNumHashesDividedByBitArraySize()
        {
            var filter = new MyTestBloomFilter<int>(250, 3);
            filter.Add(42);
            Assert.Equal(3d/250d, filter.FillRatio);
        }

        private class MyTestBloomFilter<T> : BloomFilter<T>
        {
            public MyTestBloomFilter(int bitArraySize, int numHashes)
                : base(bitArraySize, numHashes)
            {
                
            }
        }
    }
}
