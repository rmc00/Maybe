using System;
using Maybe.Utilities;

namespace Maybe.BloomFilter
{
    /// <summary>
    /// Base class for bloom filter to contain some common member variables and hashing helper functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class BloomFilterBase<T> : IBloomFilter<T>
    {
        /// <summary>
        /// The number of times an item should be hashed when being added to or checked for membership in the collection
        /// </summary>
        protected int NumberHashes;

        /// <summary>
        /// Number of buckets for storing hash info (bits or ints)
        /// </summary>
        protected readonly int CollectionLength;

        /// <summary>
        /// Creates a new bloom filter with appropriate bit width and hash functions for your expected size and error rate.
        /// </summary>
        /// <param name="expectedItems">The maximum number of items you expect to be in the bloom filter</param>
        /// <param name="acceptableErrorRate">The maximum rate of false positives you can accept. Must be a value between 0.00-1.00</param>
        /// <returns>A new bloom filter configured appropriately for number of items and error rate</returns>
        protected BloomFilterBase(int expectedItems, double acceptableErrorRate)
        {
            if (expectedItems <= 0) { throw new ArgumentException("Expected items must be at least 1.", nameof(expectedItems)); }
            if (acceptableErrorRate < 0 || acceptableErrorRate > 1) { throw new ArgumentException("Acceptable error rate must be between 0 and 1.", nameof(acceptableErrorRate)); }

            var bitWidth = (int)Math.Ceiling(expectedItems * Math.Log(acceptableErrorRate) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0)))) * 2;
            var numHashes = (int)Math.Round(Math.Log(2.0) * bitWidth / expectedItems) * 2;
            NumberHashes = numHashes;
            CollectionLength = bitWidth;
        }

        /// <summary>
        /// Protected constructor to create a new bloom filter
        /// </summary>
        /// <param name="bitArraySize">The number of bits that should be used internally to store items.</param>
        /// <param name="numberHashes">The number of times an input should be hashed before working against the internal bit array.</param>
        protected BloomFilterBase(int bitArraySize, int numberHashes)
        {
            NumberHashes = numberHashes;
            CollectionLength = bitArraySize;
        }

        /// <summary>
        /// Adds an item to the bloom filter
        /// </summary>
        /// <param name="item">The item which should be added</param>
        public abstract void Add(T item);

        /// <summary>
        /// Checks if this bloom filter currently contains an item
        /// </summary>
        /// <param name="item">The item for which to search in the bloom filter</param>
        /// <returns>False if the item is NOT in the bloom filter. True if the item MIGHT be in the bloom filter.</returns>
        public abstract bool Contains(T item);

        /// <summary>
        /// Adds an item to the bloom filter and returns if it might already be contained before
        /// </summary>
        /// <param name="item">The item which should be added and searched in the bloom filter</param>
        /// <returns>False if the item was NOT in the bloom filter before. True if the item MIGHT have been in the bloom filter.</returns>
        public abstract bool AddAndCheck(T item);

        /// <summary>
        /// Represents the ratio of positions that are set in the bloom filter to the total number of positions
        /// </summary>
        public abstract double FillRatio { get; }

        /// <summary>
        /// Hashes the <paramref name="item"/> provided and passes the hashed result to an action for processing (typically setting bits in the bit array or checking if those bits are set)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="hashAction"></param>
        protected void DoHashAction(T item, Action<int> hashAction)
        {
            var hashes = MurmurHash3.GetHashes(item, NumberHashes, CollectionLength);
            foreach (var hash in hashes)
            {
                hashAction(hash);
            }
        }
    }
}
