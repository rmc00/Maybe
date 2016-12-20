using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Maybe
{
    public class BloomFilter<T>
    {
        private readonly BitArray _collectionState;
        private readonly int _hashCount;
        private readonly HashAlgorithm _secondaryAlgo;

        /// <summary>
        /// Creates a new bloom filter collection
        /// </summary>
        /// <param name="bitArraySize">The size of the bloom filter's internal bit array. Can be configured for different tradeoffs of memory usage vs false positive rates</param>
        /// <param name="numHashes">Number of hashes to execute on each input item</param>
        public BloomFilter(int bitArraySize, int numHashes)
        {
            _collectionState = new BitArray(bitArraySize);
            _hashCount = numHashes;
            _secondaryAlgo = HashAlgorithm.Create();
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
            var secondaryHash = BitConverter.ToInt32(_secondaryAlgo.ComputeHash(ConvertToByteArray(item)), 0);

            for (var i = 0; i < _hashCount; i++)
            {
                var resultingHash = Math.Abs((primaryHash + (i * secondaryHash)) % _collectionState.Length);
                hashAction(resultingHash);
            }
        }

        private static byte[] ConvertToByteArray(object item)
        {
            if (item == null) return null;

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, item);
                return memoryStream.ToArray();
            }
        }
    }
}
