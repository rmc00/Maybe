using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FsCheck;
using FsCheck.Xunit;
using Maybe.BloomFilter;
using Xunit;

namespace Maybe.Test.BloomFilter
{
    public class CountingBloomFilterTests
    {
        [Property]
        [Trait("Category", "Property")]
        public Property Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(50, 0.02);
                filter.Add(testData);
                return filter.Contains(testData).ToProperty();
            });
        }

        [Property]
        [Trait("Category", "Property")]
        public Property Contains_WithFreshFilter_ShouldReturnFalse()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(50, 0.02);
                return (!filter.Contains(testData)).ToProperty();
            });
        }

        [Property]
        [Trait("Category", "Property")]
        public Property Contains_With5PercentFalsePositives_ShouldHaveLessThan5PercentErrors()
        {
            return Prop.ForAll(Arb.From(Gen.Choose(1, 5000)), Arb.From(Gen.Choose(1, 99)), (stepRange, errorRate) =>
            {
                var filter = new CountingBloomFilter<int>(stepRange, errorRate/100d);
                foreach (var num in Enumerable.Range(1, stepRange))
                {
                    filter.Add(num);
                }
                var errorCount = Enumerable.Range(stepRange + 1, stepRange * 2).Count(num => filter.Contains(num));
                var highError = errorRate * stepRange;
                (0 <= errorCount && errorCount <= highError).ToProperty();
            });
        }

        [Property]
        [Trait("Category", "Unit")]
        public Property Remove_WithItemNotInCollection_ShouldReturnFalse()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(100, 0.2);
                (!filter.Remove(testData)).ToProperty();
            });
        }

        [Property]
        [Trait("Category", "Property")]
        public Property Remove_WithItemInCollection_ShouldReturnTrue()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(100, 0.2);
                filter.Add(testData);
                return filter.Remove(testData).ToProperty();
            });
        }

        [Property]
        [Trait("Category", "Property")]
        public Property Remove_WithItemInCollection_ShouldRemoveItemFromCollection()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(100, 0.2);
                filter.Add(testData);
                filter.Remove(testData);
                return (!filter.Remove(testData)).ToProperty();
            });
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void FillRatio_WithNewFilter_ShouldBeZero()
        {
            var filter = new CountingBloomFilter<int>(1000, 0.05);
            Assert.Equal(0d, filter.FillRatio);
        }

        [Property]
        [Trait("Category", "Property")]
        public Property FillRatio_WithOneItem_ShouldBeNumHashesDividedByBitArraySize()
        {
            return Prop.ForAll(Arb.Default.Int32(), Arb.From(Gen.Choose(1, 10000)), Arb.From(Gen.Choose(1, 99)), (testData, bitArraySize, errorRate) =>
            {
                var realErrorRate = (int) (errorRate / 100d);
                var filter = new MyTestBloomFilter<int>(bitArraySize, realErrorRate);
                filter.Add(testData);
                return (realErrorRate / bitArraySize == filter.FillRatio).ToProperty();
            });
        }

        [Fact]
        [Trait("Category", "Unit")]
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

        [Property]
        [Trait("Category", "Property")]
        public Property AddAndCheck_WhenItemHasBeenAddedBefore_ShouldReturnTrue()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(50, 0.02);
                filter.Add(testData);
                (filter.AddAndCheck(testData)).ToProperty();
            });
        }

        [Property]
        [Trait("Category", "Property")]
        public Property AddAndCheck_WhenItemHasntBeenAddedBefore_ShouldReturnFalse()
        {
            return Prop.ForAll(Arb.Default.Int32(), testData =>
            {
                var filter = new CountingBloomFilter<int>(50, 0.02);
                (filter.AddAndCheck(testData)).ToProperty();
            });
        }

        [Fact]
        [Trait("Category", "Unit")]
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
