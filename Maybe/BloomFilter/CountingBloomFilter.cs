using System;
using System.Linq;

namespace Maybe.BloomFilter
{
    /// <summary>
    /// A bloom filter modified to store counters and allow elements to be removed from the collection.
    /// </summary>
    /// <typeparam name="T">The type of item to be stored in the collection</typeparam>
    public class CountingBloomFilter<T> : BloomFilterBase<T>
    {
        private readonly byte[] _collectionState;

        protected CountingBloomFilter(int arraySize, int numHashes) : base(arraySize, numHashes)
        {
            _collectionState = new byte[arraySize];
            for (var i = 0; i < _collectionState.Length; i++)
            {
                _collectionState[i] = 0;
            }
        }

        public override double FillRatio => _collectionState.Count(position => position > 0) / (double)_collectionState.Length;

        /// <summary>
        /// Creates a new counting bloom filter with appropriate bit width and hash functions for your expected size and error rate.
        /// </summary>
        /// <typeparam name="T">The type of item to be held in the counting bloom filter</typeparam>
        /// <param name="expectedItems">The maximum number of items you expect to be in the counting bloom filter</param>
        /// <param name="acceptableErrorRate">The maximum rate of false positives you can accept. Must be a value between 0.00-1.00</param>
        /// <returns>A new bloom filter configured appropriately for number of items and error rate</returns>
        public static CountingBloomFilter<T> Create(int expectedItems, double acceptableErrorRate)
        {
            if (expectedItems <= 0) { throw new ArgumentException("Expected items must be at least 1.", nameof(expectedItems)); }
            if (acceptableErrorRate < 0 || acceptableErrorRate > 1) { throw new ArgumentException("Acceptable error rate must be between 0 and 1.", nameof(acceptableErrorRate)); }

            var bitWidth = (int)Math.Ceiling(expectedItems * Math.Log(acceptableErrorRate) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0)))) * 2;
            var numHashes = (int)Math.Round(Math.Log(2.0) * bitWidth / expectedItems) * 2;
            return new CountingBloomFilter<T>(bitWidth, numHashes);
        }

        /// <summary>
        /// Adds an item to the counting bloom filter
        /// </summary>
        /// <param name="item">The item which should be added</param>
        public void Add(T item) => DoHashAction(item, hash => _collectionState[hash]++);

        /// <summary>
        /// Checks if this counting bloom filter currently contains an item
        /// </summary>
        /// <param name="item">The item for which to search in the bloom filter</param>
        /// <returns>False if the item is NOT in the counting bloom filter. True if the item MIGHT be in the counting bloom filter.</returns>
        public bool Contains(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash => containsItem = containsItem && _collectionState[hash] > 0);
            return containsItem;
        }

        /// <summary>
        /// Removes an item from the counting bloom filter
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the counting bloom filter might contain the item and the item was removed. False otherwise.</returns>
        public bool Remove(T item)
        {
            if (!Contains(item)) return false;

            DoHashAction(item, hash => _collectionState[hash]--);
            return true;
        }
    }
}
