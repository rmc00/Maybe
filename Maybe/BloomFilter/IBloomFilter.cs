namespace Maybe.BloomFilter
{
    public interface IBloomFilter<T>
    {
        /// <summary>
        /// Adds an item to the bloom filter
        /// </summary>
        /// <param name="item">The item which should be added</param>
        void Add(T item);

        /// <summary>
        /// Checks if this bloom filter currently contains an item
        /// </summary>
        /// <param name="item">The item for which to search in the bloom filter</param>
        /// <returns>False if the item is NOT in the bloom filter. True if the item MIGHT be in the bloom filter.</returns>
        bool Contains(T item);
    }
}
