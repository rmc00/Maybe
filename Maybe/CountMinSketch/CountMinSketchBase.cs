using System;

namespace Maybe.CountMinSketch
{
    /// <summary>
    /// An abstract class representing a general Count-Min Sketch data structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class CountMinSketchBase<T>
    {
        /// <summary>
        /// Returns the relative error, epsilon, of this <see cref="CountMinSketchBase{T}"/>
        /// </summary>
        public abstract double RelativeError { get; }
        /// <summary>
        /// Returns the confidence, delta, of this <see cref="CountMinSketchBase{T}"/>
        /// </summary>
        public abstract double Confidence { get; }
        /// <summary>
        /// Returns the depth of this <see cref="CountMinSketchBase{T}"/>
        /// </summary>
        public abstract int Depth { get; }
        /// <summary>
        /// Returns the width of this <see cref="CountMinSketchBase{T}"/>
        /// </summary>
        public abstract int Width { get; }
        /// <summary>
        /// Returns the total count of items added to this <see cref="CountMinSketchBase{T}"/>
        /// </summary>
        public abstract long TotalCount { get; }

        /// <summary>
        /// Returns the random seed that was used to initialize this <see cref="CountMinSketchBase{T}"/>
        /// </summary>
        public abstract int Seed { get; }

        /// <summary>
        /// Exposes the <see cref="CountMinSketchBase{T}"/>'s counter table
        /// </summary>
        public abstract long[,] Table { get; }

        /// <summary>
        /// Increments counters for the item
        /// </summary>
        /// <param name="item"></param>
        public abstract void Add(T item);

        /// <summary>
        /// Returns the estimated frequency of the item
        /// </summary>
        /// <param name="item">Item for which an estimated count is desired</param>
        /// <returns>Estimated frequency of this item in the <see cref="CountMinSketchBase{T}"/></returns>
        public abstract long EstimateCount(T item);

        /// <summary>
        /// Merges another <see cref="CountMinSketchBase{T}"/> with this one in place. Other must have the same depth, width, and seed to be merged.
        /// </summary>
        /// <param name="other"><see cref="CountMinSketchBase{T}"/> to be merged with this <see cref="CountMinSketchBase{T}"/></param>
        /// <returns></returns>
        public abstract CountMinSketchBase<T> MergeInPlace(CountMinSketchBase<T> other);

        /// <summary>
        /// Creates a <see cref="CountMinSketchBase{T}"/> with the given depth, width, and seed
        /// </summary>
        /// <param name="depth">The depth of the count-min sketch. Must be positive.</param>
        /// <param name="width">The width of the count-min sketch. Must be positive.</param>
        /// <param name="seed">A random seed for hashing.</param>
        /// <returns>A new <see cref="CountMinSketchBase{T}"/> created with the provided parameters</returns>
        public static CountMinSketchBase<T> Create(int depth, int width, int seed) => new CountMinSketch<T>(depth, width, seed);

        /// <summary>
        /// Creates a new <see cref="CountMinSketchBase{T}"/> with given relative error (epsilon), confidence, and random seed.
        /// </summary>
        /// <param name="epsilon">Relative error of the sketch. Must be positive.</param>
        /// <param name="confidence">Confidence of frequence estimations. Must be between 0 and 1</param>
        /// <param name="seed">A random seed for hashing.</param>
        /// <returns>A new <see cref="CountMinSketchBase{T}"/> created with the provided parameters</returns>
        public static CountMinSketchBase<T> Create(double epsilon, double confidence, int seed) => new CountMinSketch<T>(epsilon, confidence, seed);
    }
}
