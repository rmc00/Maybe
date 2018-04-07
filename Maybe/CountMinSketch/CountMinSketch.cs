using System;
using System.Linq;
using Maybe.Utilities;

namespace Maybe.CountMinSketch
{
    /// <summary>
    /// Count min sketch is a data structure that allows you to track the frequency of an item occurring within a large set. The count min sketch will never undercount items, but it can overcount by a controllable confidence interval.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CountMinSketch<T> : CountMinSketchBase<T>
    {
        private readonly MurmurHash3 _hasher = new MurmurHash3();
        private readonly int _depth;
        private readonly int _width;
        private long[,] _table;
        private long[] _hashA;
        private long _totalCount;

        /// <summary>
        /// Creates a new instance of <see cref="CountMinSketch{T}"/>
        /// </summary>
        /// <param name="depth">The number of buckets to be used for counting. More buckets will increase the probability of computing the value correctly.</param>
        /// <param name="width">The size of buckets for counting items. Wider buckets will increase accuracy but use more memory.</param>
        /// <param name="seed">Some seed for random number generation. This is passed in to allow multiple sketches to be sync'ed with the same seed.</param>
        public CountMinSketch(int depth, int width, int seed)
        {
            if (depth <= 0) { throw new ArgumentException("Depth must be a positive integer.", nameof(depth)); }
            if (width <= 0) { throw new ArgumentException("Width must be a positive integer.", nameof(width)); }

            Seed = seed;
            _depth = depth;
            _width = width;
            RelativeError = 2d / width;
            Confidence = 1 - 1 / Math.Pow(2, depth);
            InitTablesWith(depth, width, seed);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CountMinSketch{T}"/>
        /// </summary>
        /// <param name="epsilon">The accuracy of the counts produced by this data structure.</param>
        /// <param name="confidence">The probability of computing the value correctly.</param>
        /// <param name="seed">Some seed for random number generation. This is passed in to allow multiple sketches to be sync'ed with the same seed.</param>
        public CountMinSketch(double epsilon, double confidence, int seed)
        {
            if (epsilon <= 0d) { throw new ArgumentException("Relative error must be positive.", nameof(epsilon)); }
            if (confidence <= 0d || confidence >= 1d) { throw new ArgumentException("Confidence must be greater than 0 and less than 1", nameof(confidence)); }

            RelativeError = epsilon;
            Confidence = confidence;
            _width = (int)Math.Ceiling(2 / epsilon);
            _depth = (int)Math.Ceiling(-Math.Log(1 - confidence) / Math.Log(2));
            InitTablesWith(_depth, _width, seed);
        }

        private void InitTablesWith(int depth, int width, int seed)
        {
            _table = new long[depth,width];
            _hashA = new long[depth];
            var r = new Random(seed);
            for (var i = 0; i < depth; i++)
            {
                _hashA[i] = r.Next();
            }
        }

        /// <summary>
        /// Gets the seed that was used to initialize this CountMinSketch.
        /// </summary>
        public override int Seed { get; }

        /// <summary>
        /// Gets the epsilon setting used to initialize this <see cref="CountMinSketch{T}"/>.
        /// </summary>
        public override double RelativeError { get; }

        /// <summary>
        /// Gets the confidence interval used to initialize this <see cref="CountMinSketch{T}"/>
        /// </summary>
        public override double Confidence { get; }

        /// <summary>
        /// The number of buckets used for tracking items.
        /// </summary>
        public override int Depth => _depth;

        /// <summary>
        /// Gets the size of each bucket used for tracking frequency of items.
        /// </summary>
        public override int Width => _width;

        /// <summary>
        /// Gets the total number of items in this collection.
        /// </summary>
        public override long TotalCount => _totalCount;

        /// <summary>
        /// Gets or sets the table that is currently being used to track frequency of items.
        /// </summary>
        public override long[,] Table => _table;

        /// <summary>
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="item">The item to be added to the collection</param>
        public override void Add(T item) => Add(item, 1);

        /// <summary>
        /// Adds an item to the collection a specified number of times
        /// </summary>
        /// <param name="item">The item to be added</param>
        /// <param name="count">The number of times the item should be added</param>
        public void Add(T item, long count)
        {
            var buckets = GetHashBuckets(item, Depth, Width);
            for (var i = 0; i < _depth; i++)
            {
                _table[i, buckets[i]] += count;
            }
            _totalCount += count;
        }

        /// <summary>
        /// Estimates the number of times an item has been added to this <see cref="CountMinSketch{T}"/>
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>An estimated number of times that the item has been added to the collection. This will never be low but could be higher than the actual result.</returns>
        public override long EstimateCount(T item)
        {
            var res = long.MaxValue;
            var buckets = GetHashBuckets(item, Depth, Width);

            for (var i = 0; i < Depth; i++)
            {
                res = Math.Min(res, _table[i, buckets[i]]);
            }
            return res;
        }

        private int[] GetHashBuckets(T item, int hashCount, int max)
        {
            var result = new int[hashCount];
            var hashes = _hasher.GetHashes(item, hashCount, max).ToList();
            for (var i = 0; i < hashCount; i++)
            {
                result[i] = hashes[i];
            }
            return result;
        }

        /// <summary>
        /// Merges another instance of <see cref="CountMinSketch{T}"/> with this collection. The results will be an aggregate of both collections after merging.
        /// </summary>
        /// <param name="other">The <see cref="CountMinSketchBase{T}"/> that should be merged into the current collection.</param>
        /// <returns>This <see cref="CountMinSketch{T}"/> with the results from the other collection included.</returns>
        public override CountMinSketchBase<T> MergeInPlace(CountMinSketchBase<T> other)
        {
            if (other == null) { throw new IncompatibleMergeException("Cannot merge null estimator"); }
            if(other.Depth != Depth) {  throw new IncompatibleMergeException("Cannot merge estimators with different depths"); }
            if(other.Width != Width) { throw new IncompatibleMergeException("Cannot merge estimators with different widths"); }
            if(other.Seed != Seed) { throw new IncompatibleMergeException("Cannot merge sketches that were initialized with different seeds"); }

            for (var i = 0; i < _table.GetLength(0); i++)
            {
                for (var j = 0; j < _table.GetLength(1); j++)
                {
                    _table[i, j] = _table[i, j] + other.Table[i, j];
                }
            }
            _totalCount += other.TotalCount;
            return this;
        }
    }
}
