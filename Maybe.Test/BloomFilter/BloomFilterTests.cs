using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Maybe.BloomFilter;
using Xunit;

namespace Maybe.Test.BloomFilter
{
    public class BloomFilterTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = new BloomFilter<int>(50, 0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = new BloomFilter<int>(50, 0.02);
            Assert.False(filter.Contains(42));
        }

        [Theory]
        [InlineData(100, 0.05d)]
        [InlineData(1000, 0.05d)]
        [InlineData(10000, 0.05d)]
        [Trait("Category", "Unit")]
        public void Contains_With5PercentFalsePositives_ShouldHaveLessThan5PercentErrors(int stepRange, double errorRate)
        {
            var filter = new BloomFilter<int>(stepRange, errorRate);
            foreach (var num in Enumerable.Range(1, stepRange))
            {
                filter.Add(num);
            }

            var errorCount = Enumerable.Range(stepRange + 1, stepRange * 2).Count(num => filter.Contains(num));

            Assert.InRange(errorCount, 0d, errorRate * stepRange);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void FillRatio_WithNewFilter_ShouldBeZero()
        {
            var filter = new BloomFilter<int>(1000, 0.05);
            Assert.Equal(0d, filter.FillRatio);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void FillRatio_WithOneItem_ShouldBeNumHashesDividedByBitArraySize()
        {
            var filter = new MyTestBloomFilter<int>(250, 3);
            filter.Add(42);
            Assert.Equal(3d/250d, filter.FillRatio);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WhenItemHasBeenAdded_AndFilterHasBeenSerializedAndUnserialized_ShouldReturnTrue()
        {
            using (var stream = new MemoryStream())
            {
                var filterOld = new BloomFilter<int>(50, 0.02);
                filterOld.Add(42);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, filterOld);
                stream.Flush();
                stream.Position = 0;
                BloomFilter<int> filterNew = (BloomFilter<int>)formatter.Deserialize(stream);
                Assert.True(filterNew.Contains(42));
            }
        }

        private class MyTestBloomFilter<T> : BloomFilter<T>
        {
            public MyTestBloomFilter(int bitArraySize, int numHashes)
                : base(bitArraySize, numHashes)
            {

            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void AddAndCheck_WhenItemHasBeenAddedBefore_ShouldReturnTrue()
        {
            var filter = new BloomFilter<int>(50, 0.02);
            filter.Add(42);
            Assert.True(filter.AddAndCheck(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void AddAndCheck_WhenItemHasntBeenAddedBefore_ShouldReturnFalse()
        {
            var filter = new BloomFilter<int>(50, 0.02);
            Assert.False(filter.AddAndCheck(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Create_WithZeroExpectedSize_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => BloomFilter<int>.Create(0, 0.5));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Create_WithNegativeExpectedSize_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => BloomFilter<int>.Create(-100, 0.5));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Create_WithErrorRateLessThanZero_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => BloomFilter<int>.Create(100, -5));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Create_WithErrorRateGreaterThanOne_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => BloomFilter<int>.Create(100, 5));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Create_WithValidParameters_ShouldReturnBloomFilter()
        {
            var filter = BloomFilter<int>.Create(50, 0.03);
            Assert.NotNull(filter);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_WithZeroExpectedSize_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BloomFilter<int>(0, 0.5d));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_WithNegativeExpectedSize_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BloomFilter<int>(-100, 0.5d));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_WithErrorRateLessThanZero_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BloomFilter<int>(100, -5d));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_WithErrorRateGreaterThanOne_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BloomFilter<int>(100, 5d));
        }
    }
}
