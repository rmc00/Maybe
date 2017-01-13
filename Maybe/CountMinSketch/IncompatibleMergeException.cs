using System;

namespace Maybe.CountMinSketch
{
    public class IncompatibleMergeException : Exception
    {
        public IncompatibleMergeException() {  }

        public IncompatibleMergeException(string message) : base(message) { }

        public IncompatibleMergeException(string message, Exception inner) : base(message, inner) {  }
    }
}
