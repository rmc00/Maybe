using System;
using System.Collections.Generic;
using System.IO;

namespace Maybe.Utilities
{
    [Serializable]
    internal class MurmurHash3
    {
        private readonly uint _seed;

        public MurmurHash3()
        {
            _seed = (uint) DateTime.Now.Ticks;
        }

        public MurmurHash3(uint seed)
        {
            _seed = seed;
        }

        /// <summary>
        /// Uses Dillinger and Manolios algorithm to calculate many hashes from 2 main hash functions (built-in .NET hash and Murmur3)
        /// </summary>
        /// <param name="item">Item to hash</param>
        /// <param name="numHashes">Desired number of hashes to computer</param>
        /// <param name="maxHashValue">Maximum value that will be returned; modulus is used to limit results</param>
        /// <returns></returns>
        public IEnumerable<int> GetHashes(object item, int numHashes, int maxHashValue)
        {
            var primaryHash = item.GetHashCode();
            int secondaryHash;
            using (var memoryStream = new MemoryStream(ByteConverter.ConvertToByteArray(item)))
            {
                secondaryHash = Hash(memoryStream);
            }

            for (var i = 0; i < numHashes; i++)
            {
                yield return Math.Abs((primaryHash + i * secondaryHash) % maxHashValue);
            }
        }

        /// <summary>
        /// Maps a stream of data to an integer hash
        /// </summary>
        /// <param name="stream">Stream of data to hash</param>
        /// <returns>Int hash</returns>
        public int Hash(Stream stream)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            var h1 = _seed;
            uint streamLength = 0;
            using (var reader = new BinaryReader(stream))
            {
                var chunk = reader.ReadBytes(4);
                while (chunk.Length > 0)
                {
                    streamLength += (uint)chunk.Length;
                    uint k1;
                    switch (chunk.Length)
                    {
                        case 4:
                            /* Get four bytes from the input into an uint */
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16
                              | chunk[3] << 24);

                            /* bitmagic hash */
                            k1 *= c1;
                            k1 = Rotl32(k1, 15);
                            k1 *= c2;

                            h1 ^= k1;
                            h1 = Rotl32(h1, 13);
                            h1 = h1 * 5 + 0xe6546b64;
                            break;
                        case 3:
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16);
                            k1 *= c1;
                            k1 = Rotl32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                        case 2:
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8);
                            k1 *= c1;
                            k1 = Rotl32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                        case 1:
                            k1 = chunk[0];
                            k1 *= c1;
                            k1 = Rotl32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;

                    }
                    chunk = reader.ReadBytes(4);
                }
            }

            // finalization, magic chants to wrap it all up
            h1 ^= streamLength;
            h1 = Fmix(h1);

            unchecked //ignore overflow
            {
                return (int)h1;
            }
        }

        private static uint Rotl32(uint x, byte r) => (x << r) | (x >> (32 - r));

        private static uint Fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
    }
}
