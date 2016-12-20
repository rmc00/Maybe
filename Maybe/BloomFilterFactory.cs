using System;

namespace Maybe
{
    public class BloomFilterFactory
    {
        /// <summary>
        /// Creates a new bloom filter with appropriate bit width and hash functions for your expected size and error rate.
        /// </summary>
        /// <typeparam name="T">The type of item to be held in the bloom filter</typeparam>
        /// <param name="expectedItems">The maximum number of items you expect to be in the bloom filter</param>
        /// <param name="acceptableErrorRate">The maximum rate of false positives you can accept. Must be a value between 0.00-1.00</param>
        /// <returns>A new bloom filter configured appropriately for number of items and error rate</returns>
        public static BloomFilter<T> Create<T>(int expectedItems, double acceptableErrorRate)
        {
            if(expectedItems <= 0) { throw new ArgumentException("Expected items must be at least 1.", nameof(expectedItems)); }
            if(acceptableErrorRate < 0 || acceptableErrorRate > 1) { throw new ArgumentException("Acceptable error rate must be between 0 and 1.", nameof(acceptableErrorRate)); }

            var bitWidth = (int)Math.Ceiling(-expectedItems*Math.Log(acceptableErrorRate)/Math.Pow(Math.Log(2), 2));
            var numHashes = (int)Math.Ceiling(bitWidth/expectedItems*Math.Log(2));
            return new BloomFilter<T>(bitWidth, numHashes);
        }
    }
}
