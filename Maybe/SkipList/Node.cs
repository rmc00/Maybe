using System;

namespace Maybe.SkipList
{
    /// <summary>
    /// Represents a single node on a SkipList -- Contains a value and a set of follow up nodes at various levels.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Node<T>
    {
        /// <summary>
        /// Nodes that follow this current node at a given level (where the array index is the level)
        /// </summary>
        public Node<T>[] Next { get; }

        /// <summary>
        /// The value of this node.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Creates a new instance of this node.
        /// </summary>
        /// <param name="value">The value of the node.</param>
        /// <param name="level">The level where the node is stored in the <see cref="SkipList{T}"/> tree</param>
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
