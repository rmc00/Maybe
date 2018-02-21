﻿using System;
using Maybe.Utilities;

namespace Maybe.BloomFilter
{
    [Serializable]
    public abstract class BloomFilterBase<T> : IBloomFilter<T>
    {
        protected int NumberHashes;
        private readonly int _collectionLength;
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
