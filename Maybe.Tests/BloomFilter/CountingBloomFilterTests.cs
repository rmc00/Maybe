using System.Linq;
using Maybe.BloomFilter;
using Xunit;

namespace Maybe.Tests.BloomFilter
{
    public class CountingBloomFilterTests
    {
        [Fact]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = CountingBloomFilter<int>.Create(50, 0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = CountingBloomFilter<int>.Create(50, 0.02);
            Assert.False(filter.Contains(42));
        }

        [Theory]
        [InlineData(100, 0.05d)]
        [InlineData(1000, 0.05d)]
        [InlineData(10000, 0.05d)]
        public void Contains_With5PercentFalsePositives_ShouldHaveLessThan5PercentErrors(int stepRange, double errorRate)
        {
            var filter = CountingBloomFilter<int>.Create(stepRange, errorRate);
            foreach (var num in Enumerable.Range(1, stepRange))
            {
                filter.Add(num);
            }

            var errorCount = Enumerable.Range(stepRange + 1, stepRange * 2).Count(num => filter.Contains(num));

            Assert.InRange(errorCount, 0d, errorRate * stepRange);
        }

        [Fact]
        public void Remove_WithItemNotInCollection_ShouldReturnFalse()
        {
            var filter = CountingBloomFilter<int>.Create(100, 0.2);
            Assert.False(filter.Remove(27));
        }

        [Fact]
        public void Remove_WithItemInCollection_ShouldReturnTrue()
        {
            var filter = CountingBloomFilter<int>.Create(100, 0.2);
            filter.Add(27);
            Assert.True(filter.Remove(27));
        }

        [Fact]
        public void Remove_WithItemInCollection_ShouldRemoveItemFromCollection()
        {
            var filter = CountingBloomFilter<int>.Create(100, 0.2);
            filter.Add(27);
            filter.Remove(27);
            Assert.False(filter.Contains(27));
        }

		[Fact]
        public void Add_WithCounterAtMaxValue_ShouldRemainConstant()
        {
            var filter = CountingBloomFilter<int>.Create(50, 0.01);
            while(filter.CounterAt(42) < byte.MaxValue)
            {
                filter.Add(42);
            }
            filter.Add(42); // one additional add to attempt to roll over byte.maxvalue
            Assert.True(filter.Contains(42));
        }
    }
}
