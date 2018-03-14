using System;
using System.Collections.Generic;
using System.Linq;

namespace Maybe.BloomFilter
{
    /// <summary>
    /// Represents a composite bloom filter, which will create many internal bloom filters to hold more values without increasing expected error rate.
    /// </summary>
    [Serializable]
    public class ScalableBloomFilter<T> : IBloomFilter<T>
    {
        /// <summary>
        /// The minimum number of items that this scalable bloom filter will handle.
        /// </summary>
        public const int MinimumCapacity = 50;

        private IEnumerable<BloomFilterBase<T>> _filters;
        private readonly double _maxErrorRate;
        private int _activeItemCount;
        private int _capacity;

        /// <summary>
        /// Creates a new bloom filter with error rate limited to the desired ratio.
        /// </summary>
        /// <param name="maximumErrorRate">Maximum error rate to tolerate -- more memory will be used to reduce error rate.</param>
        public ScalableBloomFilter(double maximumErrorRate)
        {
            _maxErrorRate = maximumErrorRate;
        }

        /// <summary>
        /// Adds a new item to the bloom filter and scales the bloom filter as needed.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (_activeItemCount >= _capacity)
            {
                _capacity = Math.Max(MinimumCapacity, _capacity * 2);
                _filters = AddNewFilter(_maxErrorRate, _capacity, _filters);
                _activeItemCount = 0;
            }
            _activeItemCount++;
            _filters.Last().Add(item);
        }

        /// <summary>
        /// Checks whether an item may currently exist in the bloom filter.
        /// </summary>
        /// <param name="item">The item to check for membership in this <see cref="ScalableBloomFilter{T}"/></param>
        /// <returns>True if the item MIGHT be in the collection. False if the item is NOT in the collection.</returns>
        public bool Contains(T item) => _filters != null && _filters.Any(filter => filter.Contains(item));

        /// <summary>
        /// Gets the number of filters that are currently being used internally to hold items without exceeding the error rate.
        /// </summary>
        public int NumberFilters => _filters.Count();

        private static IEnumerable<BloomFilterBase<T>> AddNewFilter(double maxError, int capacity, IEnumerable<BloomFilterBase<T>> currentFilters)
        {
            var filters = (currentFilters ?? new List<BloomFilterBase<T>>()).ToList();
            filters.Add(new BloomFilter<T>(capacity, maxError));
            return filters;
        }
    }
}
