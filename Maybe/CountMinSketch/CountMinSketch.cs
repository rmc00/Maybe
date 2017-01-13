using System;
using System.IO;
using System.Text;
using Maybe.Hashing;
using Newtonsoft.Json;

namespace Maybe.CountMinSketch
{
    public class CountMinSketch<T> : CountMinSketchBase<T>
    {
        private readonly int _depth;
        private readonly int _width;
        private long[,] _table;
        private long[] _hashA;
        private long _totalCount;

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

        public override int Seed { get; }

        public override double RelativeError { get; }

        public override double Confidence { get; }

        public override int Depth => _depth;

        public override int Width => _width;

        public override long TotalCount => _totalCount;

        public override long[,] Table => _table;

        public override void Add(T item) => Add(item, 1);

        public void Add(T item, long count)
        {
            var buckets = GetHashBuckets(item, Depth, Width);
            for (var i = 0; i < _depth; i++)
            {
                _table[i, buckets[i]] += count;
            }
            _totalCount += count;
        }

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

        private static int[] GetHashBuckets(T item, int hashCount, int max)
        {
            var result = new int[hashCount];
            var hash1 = item.GetHashCode();
            int hash2;
            using (var memoryStream = new MemoryStream(ConvertToByteArray(item)))
            {
                hash2 = MurmurHash3.Hash(memoryStream);
            }

            for (var i = 0; i < hashCount; i++)
            {
                result[i] = Math.Abs((hash1 + i*hash2)%max);
            }
            return result;
        }

        

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

        private static byte[] ConvertToByteArray(object item) => item == null ? null : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
    }
}
