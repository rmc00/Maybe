using System;

namespace Maybe.CountMinSketch
{
    /// <summary>
    /// Represent an error encountered when merging <see cref="CountMinSketchBase{T}"/>
    /// </summary>
    public class IncompatibleMergeException : Exception
    {
        /// <summary>
        /// Creates a new instance of this exception.
        /// </summary>
        public IncompatibleMergeException() {  }

        /// <summary>
        /// Creates a new instance of this exception with a custom error message
        /// </summary>
        /// <param name="message">The message to be set on the exception</param>
        public IncompatibleMergeException(string message) : base(message) { }

        /// <summary>
        /// Creates a new instance of this exception with a custom error message and an inner exception for context.
        /// </summary>
        /// <param name="message">The message to be set on the exception.</param>
        /// <param name="inner">The inner exception to be included on the exception.</param>
        public IncompatibleMergeException(string message, Exception inner) : base(message, inner) {  }
    }
}
