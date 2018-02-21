using System;

namespace Maybe.SkipList
{
    [Serializable]
    public class Node<T>
    {
        public Node<T>[] Next { get; }
        public T Value { get; }

        public Node(T value, int level)
        {
            if(level < 0) { throw new ArgumentException("Level must be >= 0!", nameof(level)); }
            Value = value;
            Next = new Node<T>[level];
        }

        /// <summary>
        /// Checks if this node has a link to the next node at a given level
        /// </summary>
        /// <param name="level">The level of link to search for</param>
        /// <returns>True if the node has a link to another node at that level. Otherwise false.</returns>
        public bool HasNextAtLevel(int level)
        {
            if (level < 0) { throw new ArgumentException("Level must be >= 0!", nameof(level)); }
            return level < Next.Length && Next[level] != null;
        }
    }
}
