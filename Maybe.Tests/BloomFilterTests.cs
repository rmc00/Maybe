using System.ComponentModel;
using System.Linq;
using Xunit;

namespace Maybe.Tests
{
    public class BloomFilterTests
    {
        [Fact, Category("Unit")]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = BloomFilter<int>.Create(50, 0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact, Category("Unit")]
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
    }
}
