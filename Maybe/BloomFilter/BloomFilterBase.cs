using System;
using System.IO;
using System.Text;
using Maybe.Utilities;
using Newtonsoft.Json;

namespace Maybe.BloomFilter
{
    public abstract class BloomFilterBase<T>
    {
        protected int NumberHashes;
        private readonly int _collectionLength;
        protected BloomFilterBase(int bitArraySize, int numberHashes)
        {
            NumberHashes = numberHashes;
            _collectionLength = bitArraySize;
        }

        /// <summary>
        /// Represents the ratio of positions that are set in the bloom filter to the total number of positions
        /// </summary>
        public abstract double FillRatio { get; }

        protected void DoHashAction(T item, Action<int> hashAction)
        {
            var primaryHash = item.GetHashCode();
            int secondaryHash;
            using (var memoryStream = new MemoryStream(ConvertToByteArray(item)))
            {
                secondaryHash = MurmurHash3.Hash(memoryStream);
            }

            for (var i = 0; i < NumberHashes; i++)
            {
                var resultingHash = Math.Abs((primaryHash + i * secondaryHash) % _collectionLength);
                hashAction(resultingHash);
            }
        }

        private static byte[] ConvertToByteArray(object item) => item == null ? null : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
    }
}
