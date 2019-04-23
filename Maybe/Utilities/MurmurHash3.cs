using System;
using System.Collections.Generic;

namespace Maybe.Utilities
{
    [Serializable]
    internal class MurmurHash3
    {
        /// <summary>
        /// Uses Dillinger and Manolios algorithm to calculate many hashes from 2 main hash functions (built-in .NET hash and Murmur3)
        /// </summary>
        /// <param name="item">Item to hash</param>
        /// <param name="numHashes">Desired number of hashes to computer</param>
        /// <param name="maxHashValue">Maximum value that will be returned; modulus is used to limit results</param>
        /// <returns></returns>
        public IEnumerable<int> GetHashes(byte[] item, int numHashes, int maxHashValue)
        {
            var primaryHash = Hash(item, 293);
            var secondaryHash = Hash(item, 697);

            for (var i = 0; i < numHashes; i++)
            {
                yield return Math.Abs((primaryHash + i * secondaryHash) % maxHashValue);
            }
        }

        /// <summary>
        /// Maps a stream of data to an integer hash
        /// </summary>
        /// <param name="bytes">Stream of data to hash</param>
        /// <param name="seed">Seed used for hashing functions</param>
        /// <returns>Int hash</returns>
        public int Hash(byte[] bytes, uint seed)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            var h1 = seed;

            var stream = new Span<byte>(bytes);

            int index;
            for (index = 0; index < stream.Length-4; index += 4)
            {
                var slice = stream.Slice(index, 4);
                uint k1;
                /* Get four bytes from the input into an uint */
                k1 = (uint)
                    (slice[0]
                     | slice[1] << 8
                     | slice[2] << 16
                     | slice[3] << 24);

                /* bitmagic hash */
                k1 *= c1;
                k1 = Rotl32(k1, 15);
                k1 *= c2;

                h1 ^= k1;
                h1 = Rotl32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            // handle remainder
            if (index < stream.Length)
            {
                var slice = stream.Slice(index, stream.Length-index);
                uint k1;
                switch (slice.Length)
                {
                    case 3:
                        k1 = (uint)
                            (slice[0]
                             | slice[1] << 8
                             | slice[2] << 16);
                        k1 *= c1;
                        k1 = Rotl32(k1, 15);
                        k1 *= c2;
                        h1 ^= k1;
                        break;
                    case 2:
                        k1 = (uint)
                            (slice[0]
                             | slice[1] << 8);
                        k1 *= c1;
                        k1 = Rotl32(k1, 15);
                        k1 *= c2;
                        h1 ^= k1;
                        break;
                    case 1:
                        k1 = slice[0];
                        k1 *= c1;
                        k1 = Rotl32(k1, 15);
                        k1 *= c2;
                        h1 ^= k1;
                        break;
                }
            }

            // finalization, magic chants to wrap it all up
            h1 ^= (uint)stream.Length;
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
