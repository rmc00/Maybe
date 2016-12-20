using System.ComponentModel;
using Xunit;

namespace Maybe.Tests
{
    public class BloomFilterTests
    {
        [Fact, Category("Unit")]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = new BloomFilter<int>(100, 2);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact, Category("Unit")]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = new BloomFilter<int>(20, 1);
            Assert.False(filter.Contains(42));
        }
    }
}
