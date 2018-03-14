using System;

namespace Maybe.CountMinSketch
{
    /// <summary>
    /// Represent an error encountered when merging <see cref="CountMinSketchBase{T}"/>
    /// </summary>
    public class IncompatibleMergeException : Exception
    {
        /// <summary>
        /// Creates a new instance of this exception with a custom error message
        /// </summary>
        /// <param name="message">The message to be set on the exception</param>
        public IncompatibleMergeException(string message) : base(message) { }
    }
}
