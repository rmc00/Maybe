using System;
using System.Collections.Generic;
using System.Linq;

namespace Maybe.BloomFilter
{
    /// <summary>
    /// Represents a composite bloom filter, which will create many internal bloom filters to hold more values without increasing expected error rate.
    /// </summary>
    public class ScalableBloomFilter<T> : IBloomFilter<T>
    {
        public const int MinimumCapacity = 50;

        private IEnumerable<BloomFilterBase<T>> _filters;
        private readonly double _maxErrorRate;
        private int _activeItemCount;
        private int _capacity;

        public ScalableBloomFilter(double maximumErrorRate)
        {
            _maxErrorRate = maximumErrorRate;
        }

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

        public bool Contains(T item) => _filters != null && _filters.Any(filter => filter.Contains(item));

        public int NumberFilters => _filters.Count();

        private static IEnumerable<BloomFilterBase<T>> AddNewFilter(double maxError, int capacity, IEnumerable<BloomFilterBase<T>> currentFilters)
        {
            var filters = (currentFilters ?? new List<BloomFilterBase<T>>()).ToList();
            filters.Add(BloomFilter<T>.Create(capacity, maxError));
            return filters;
        }
    }
}
