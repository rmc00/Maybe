using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Xunit.Assert;

namespace Maybe.Tests
{
    public class BloomFilterFactoryTests
    {
        [Fact]
        public void Create_WithAcceptableValues_ShouldReturnBloomFilter()
        {
            var filter = BloomFilterFactory.Create<int>(100, 0.05);
            Assert.NotNull(filter);
        }

        [Fact]
        public void Create_WithNegativeNumberItems_ShouldThrowException() => Assert.Throws<ArgumentException>(() => BloomFilterFactory.Create<int>(-5, 0.05));

        [Fact]
        public void Create_WithNegativeErrorRate_ShouldThrowException() => Assert.Throws<ArgumentException>(() => BloomFilterFactory.Create<int>(100, -0.05));

        [Fact]
        public void Create_WithErrorRateOverOne_ShouldThrowException() => Assert.Throws<ArgumentException>(() => BloomFilterFactory.Create<int>(100, 1.05));
    }
}
