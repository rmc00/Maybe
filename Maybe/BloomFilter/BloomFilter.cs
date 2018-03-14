using System;
using System.Collections;
using System.Linq;

namespace Maybe.BloomFilter
{
    /// <summary>
    /// Basic bloom filter collection
    /// </summary>
    /// <typeparam name="T">Type of data that will be contained in the bloom filter.</typeparam>
    [Serializable]
    public class BloomFilter<T> : BloomFilterBase<T>
    {
        private readonly BitArray _collectionState;

        /// <inheritdoc />
        protected BloomFilter(int bitArraySize, int numHashes) : base(bitArraySize, numHashes)
        {
            _collectionState = new BitArray(bitArraySize, false);
        }

        /// <inheritdoc />
        public BloomFilter(int expectedItems, double acceptableErrorRate) : base(expectedItems, acceptableErrorRate)
        {
            _collectionState = new BitArray(CollectionLength, false);
        }

        /// <summary>
        /// Gets the ratio of how many bits in the bloom filter are set to the total number of bits. When this ratio is too high, the chance for error increases.
        /// </summary>
        public override double FillRatio => _collectionState.Cast<bool>().Count(bit => bit) / (double)_collectionState.Length;

        /// <summary>
        /// Creates a new bloom filter with appropriate bit width and hash functions for your expected size and error rate.
        /// </summary>
        /// <param name="expectedItems">The maximum number of items you expect to be in the bloom filter</param>
        /// <param name="acceptableErrorRate">The maximum rate of false positives you can accept. Must be a value between 0.00-1.00</param>
        /// <returns>A new bloom filter configured appropriately for number of items and error rate</returns>
        public static BloomFilter<T> Create(int expectedItems, double acceptableErrorRate)
        {
            return new BloomFilter<T>(expectedItems, acceptableErrorRate);
        }

        /// <summary>
        /// Adds an item to the bloom filter
        /// </summary>
        /// <param name="item">The item which should be added</param>
        public override void Add(T item) => DoHashAction(item, hash => _collectionState[hash] = true);

        /// <summary>
        /// Adds an item to the bloom filter and returns if it might already be contained before
        /// </summary>
        /// <param name="item">The item which should be added and searched in the bloom filter</param>
        /// <returns>False if the item was NOT in the bloom filter before. True if the item MIGHT have been in the bloom filter.</returns>
        public override bool AddAndCheck(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash =>
            {
                containsItem = containsItem && _collectionState[hash];
                _collectionState[hash] = true;
            });
            return containsItem;
        }

        /// <summary>
        /// Checks if this bloom filter currently contains an item
        /// </summary>
        /// <param name="item">The item for which to search in the bloom filter</param>
        /// <returns>False if the item is NOT in the bloom filter. True if the item MIGHT be in the bloom filter.</returns>
        public override bool Contains(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash => containsItem = containsItem && _collectionState[hash]);
            return containsItem;
        }
    }
}
