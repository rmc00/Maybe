using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Maybe.BloomFilter;
using Xunit;

namespace Maybe.Test.BloomFilter
{
    public class CountingBloomFilterTests
    {
        [Fact]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = new CountingBloomFilter<int>(50, 0.02);
            filter.Add(42);
            Assert.True(filter.Contains(42));
        }

        [Fact]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = new CountingBloomFilter<int>(50, 0.02);
            Assert.False(filter.Contains(42));
        }

        [Theory]
        [InlineData(100, 0.05d)]
        [InlineData(1000, 0.05d)]
        [InlineData(10000, 0.05d)]
        public void Contains_With5PercentFalsePositives_ShouldHaveLessThan5PercentErrors(int stepRange, double errorRate)
        {
            var filter = new CountingBloomFilter<int>(stepRange, errorRate);
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
            var filter = new CountingBloomFilter<int>(100, 0.2);
            Assert.False(filter.Remove(27));
        }

        [Fact]
        public void Remove_WithItemInCollection_ShouldReturnTrue()
        {
            var filter = new CountingBloomFilter<int>(100, 0.2);
            filter.Add(27);
            Assert.True(filter.Remove(27));
        }

        [Fact]
        public void Remove_WithItemInCollection_ShouldRemoveItemFromCollection()
        {
            var filter = new CountingBloomFilter<int>(100, 0.2);
            filter.Add(27);
            filter.Remove(27);
            Assert.False(filter.Contains(27));
        }

        [Fact]
        public void FillRatio_WithNewFilter_ShouldBeZero()
        {
            var filter = new CountingBloomFilter<int>(1000, 0.05);
            Assert.Equal(0d, filter.FillRatio);
        }

        [Fact]
        public void FillRatio_WithOneItem_ShouldBeNumHashesDividedByBitArraySize()
        {
            var filter = new MyTestBloomFilter<int>(250, 3);
            filter.Add(42);
            Assert.Equal(3d / 250d, filter.FillRatio);
        }

        [Fact]
        public void Add_WithCounterAtMaxValue_ShouldRemainConstant()
        {
            var filter = new CountingBloomFilter<int>(50, 0.01);
            while(filter.CounterAt(42) < byte.MaxValue)
            {
                filter.Add(42);
            }
            filter.Add(42); // one additional add to attempt to roll over byte.maxvalue
            Assert.True(filter.Contains(42));
        }

        [Fact]
        public void AddAndCheck_WhenItemHasBeenAddedBefore_ShouldReturnTrue()
        {
            var filter = new CountingBloomFilter<int>(50, 0.02);
            filter.Add(42);
            Assert.True(filter.AddAndCheck(42));
        }

        [Fact]
        public void AddAndCheck_WhenItemHasntBeenAddedBefore_ShouldReturnFalse()
        {
            var filter = new CountingBloomFilter<int>(50, 0.02);
            Assert.False(filter.AddAndCheck(42));
        }

        [Fact]
        public void Contains_WhenItemHasBeenAdded_AndFilterHasBeenSerializedAndUnserialized_ShouldReturnTrue()
        {
            using (var stream = new MemoryStream())
            {
                var filterOld = new CountingBloomFilter<int>(50, 0.02);
                filterOld.Add(42);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, filterOld);
                stream.Flush();
                stream.Position = 0;
                CountingBloomFilter<int> filterNew = (CountingBloomFilter<int>)formatter.Deserialize(stream);
                Assert.True(filterNew.Contains(42));
            }
        }

        private class MyTestBloomFilter<T> : CountingBloomFilter<T>
        {
            public MyTestBloomFilter(int bitArraySize, int numHashes)
                : base(bitArraySize, numHashes)
            {

            }
        }
    }
}
