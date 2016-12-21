using System;
using System.Collections;
using System.IO;
using System.Text;
using Maybe.Utilities;
using Newtonsoft.Json;

namespace Maybe
{
    public class BloomFilter<T>
    {
        private readonly BitArray _collectionState;
        private readonly int _hashCount;

        private BloomFilter(int bitArraySize, int numHashes)
        {
            _collectionState = new BitArray(bitArraySize, false);
            _hashCount = numHashes;
        }

        /// <summary>
        /// Creates a new bloom filter with appropriate bit width and hash functions for your expected size and error rate.
        /// </summary>
        /// <typeparam name="T">The type of item to be held in the bloom filter</typeparam>
        /// <param name="expectedItems">The maximum number of items you expect to be in the bloom filter</param>
        /// <param name="acceptableErrorRate">The maximum rate of false positives you can accept. Must be a value between 0.00-1.00</param>
        /// <returns>A new bloom filter configured appropriately for number of items and error rate</returns>
        public static BloomFilter<T> Create(int expectedItems, double acceptableErrorRate)
        {
            if (expectedItems <= 0) { throw new ArgumentException("Expected items must be at least 1.", nameof(expectedItems)); }
            if (acceptableErrorRate < 0 || acceptableErrorRate > 1) { throw new ArgumentException("Acceptable error rate must be between 0 and 1.", nameof(acceptableErrorRate)); }

            var bitWidth = (int)Math.Ceiling(expectedItems * Math.Log(acceptableErrorRate) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0)))) * 2;
            var numHashes = (int)Math.Round(Math.Log(2.0) * bitWidth / expectedItems) * 2;
            return new BloomFilter<T>(bitWidth, numHashes);
        }

        /// <summary>
        /// Adds an item to the bloom filter
        /// </summary>
        /// <param name="item">The item which should be added</param>
        public void Add(T item) => DoHashAction(item, hash => _collectionState[hash] = true);

        /// <summary>
        /// Checks if this bloom filter currently contains an item
        /// </summary>
        /// <param name="item">The item for which to search in the bloom filter</param>
        /// <returns>False if the item is NOT in the bloom filter. True if the item MIGHT be in the bloom filter.</returns>
        public bool Contains(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash => containsItem = containsItem && _collectionState[hash]);
            return containsItem;
        }

        private void DoHashAction(T item, Action<int> hashAction)
        {
            var primaryHash = item.GetHashCode();
            int secondaryHash;
            using (var memoryStream = new MemoryStream(ConvertToByteArray(item)))
            {
                secondaryHash = MurmurHash3.Hash(memoryStream);
            }

            for (var i = 0; i < _hashCount; i++)
            {
                var resultingHash = Math.Abs((primaryHash + i * secondaryHash) % _collectionState.Length);
                hashAction(resultingHash);
            }
        }

        private static byte[] ConvertToByteArray(object item) => item == null ? null : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
    }
}
