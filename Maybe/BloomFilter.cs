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
        public BloomFilter(int bitArraySize, int numHashes)
        {
            _collectionState = new BitArray(bitArraySize);
            _hashCount = numHashes;
            _secondaryAlgo = HashAlgorithm.Create();
        }

        public void Add(T item)
        {
            DoHashAction(item, hash => _collectionState[hash] = true);
        }

        public bool Contains(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash => containsItem = containsItem && _collectionState[hash]);
            return containsItem;
        }

        private byte[] ConvertToByteArray(object item)
        {
            if (item == null) return null;

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, item);
                return memoryStream.ToArray();
            }
        }

        private void DoHashAction(T item, Action<int> hashAction)
        {
            var primaryHash = item.GetHashCode();
            var secondaryHash = BitConverter.ToInt32(_secondaryAlgo.ComputeHash(ConvertToByteArray(item)), 0);

            for (int i = 0; i < _hashCount; i++)
            {
                int resultingHash = Math.Abs((primaryHash + (i * secondaryHash)) % _collectionState.Length);
                hashAction(resultingHash);
            }
        }
    }
}
