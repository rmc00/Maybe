using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maybe.Tests
{
    [TestClass]
    public class BloomFilterTests
    {
        [TestMethod]
        public void Contains_WhenItemHasBeenAdded_ShouldReturnTrue()
        {
            var filter = new BloomFilter<int>(100, 2);
            filter.Add(42);
            Assert.IsTrue(filter.Contains(42));
        }

        [TestMethod]
        public void Contains_WithFreshFilter_ShouldReturnFalse()
        {
            var filter = new BloomFilter<int>(20, 1);
            Assert.IsFalse(filter.Contains(42));
        }
    }
}
