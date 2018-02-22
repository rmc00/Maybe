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
        private readonly int _collectionLength;

        /// <summary>
        /// Protected constructor to create a new bloom filter
        /// </summary>
        /// <param name="bitArraySize">The number of bits that should be used internally to store items.</param>
        /// <param name="numberHashes">The number of times an input should be hashed before working against the internal bit array.</param>
        protected BloomFilterBase(int bitArraySize, int numberHashes)
        {
            NumberHashes = numberHashes;
            _collectionLength = bitArraySize;
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
            var hashes = MurmurHash3.GetHashes(item, NumberHashes, _collectionLength);
            foreach (var hash in hashes)
            {
                hashAction(hash);
            }
        }
    }
}
