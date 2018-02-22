using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Maybe.SkipList
{
    /// <summary>
    /// A sorted collection which allows for fast search by creating hierarchies of links between nodes
    /// </summary>
    /// <typeparam name="T">The type to be contained in the <see cref="SkipList{T}"/></typeparam>
    [Serializable]
    public class SkipList<T> : IEnumerable<T>, ISerializable
    {
        private readonly Node<T> _headNode = new Node<T>(default(T), 33); // The max. number of levels is 33
        private readonly Random _randomGenerator = new Random();
        private int _levels = 1;
        private readonly IComparer<T> _comparer = Comparer<T>.Default;

        /// <summary>
        /// Creates a new <see cref="SkipList{T}"/>
        /// </summary>
        /// <param name="comparer">An optional comparer for sorting values. If null the default <see cref="Comparer{T}"/> will be used.</param>
        public SkipList(IComparer<T> comparer=null)
        {
            if (comparer != null)
            {
                _comparer = comparer;
            }
        }

        /// <summary>
        /// Protected constructor used to deserialize an instance of <see cref="SkipList{T}"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SkipList(SerializationInfo info, StreamingContext context)
        {
            _headNode = (Node<T>) info.GetValue("headNode", typeof(Node<T>));
            _levels = info.GetInt32("levels");
            _comparer = (IComparer<T>)info.GetValue("comparer", typeof(IComparer<T>));
        }

        /// <summary>
        /// Adds the value to the skip list
        /// </summary>
        public void Add(T value)
        {
            var level = 0;
            for (var r = _randomGenerator.Next(); (r & 1) == 1; r >>= 1)
            {
                level++;
                if (level == _levels)
                {
                    _levels++;
                    break;
                }
            }

            var addNode = new Node<T>(value, level + 1);
            var currentNode = _headNode;
            for (var currentLevel = _levels - 1; currentLevel >= 0; currentLevel--)
            {
                while (currentNode.HasNextAtLevel(currentLevel))
                {
                    if (_comparer.Compare(currentNode.Next[currentLevel].Value, value) == 1)
                    {
                        // current value has skipped over the needed position, need to drop down a level and look there
                        break;
                    }
                    currentNode = currentNode.Next[currentLevel];
                }

                if (currentLevel <= level)
                {
                    // add the node here
                    addNode.Next[currentLevel] = currentNode.Next[currentLevel];
                    currentNode.Next[currentLevel] = addNode;
                }
            }
        }

        /// <summary>
        /// Adds multiple values to the collection
        /// </summary>
        /// <param name="values">A collection of values which should all be added</param>
        public void AddRange(IEnumerable<T> values)
        {
            if (values == null) return;
            foreach (var value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// Returns whether a particular value already exists in the skip list
        /// </summary>
        public bool Contains(T value)
        {
            var currentNode = _headNode;
            for (var currentLevel = _levels - 1; currentLevel >= 0; currentLevel--)
            {
                while (currentNode.HasNextAtLevel(currentLevel))
                {
                    if (_comparer.Compare(currentNode.Next[currentLevel].Value, value) == 1) break;
                    if (_comparer.Compare(currentNode.Next[currentLevel].Value, value) == 0) return true;
                    currentNode = currentNode.Next[currentLevel];
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to remove one occurence of a particular value from the skip list. Returns
        /// whether the value was found in the skip list.
        /// </summary>
        public bool Remove(T value)
        {
            var currentNode = _headNode;

            var found = false;
            for (var currentLevel = _levels - 1; currentLevel >= 0; currentLevel--)
            {
                while (currentNode.HasNextAtLevel(currentLevel))
                {
                    if (_comparer.Compare(currentNode.Next[currentLevel].Value, value) == 0)
                    {
                        found = true;
                        currentNode.Next[currentLevel] = currentNode.Next[currentLevel].Next[currentLevel];
                        break;
                    }

                    if (_comparer.Compare(currentNode.Next[currentLevel].Value, value) == 1) { break; }

                    currentNode = currentNode.Next[currentLevel];
                }
            }

            return found;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerates all nodes of this collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var currentNode = _headNode.Next[0];
            do
            {
                yield return currentNode.Value;
                currentNode = currentNode.Next[0];
            } while (currentNode != null && currentNode.HasNextAtLevel(0));

            if (currentNode != null)
            {
                yield return currentNode.Value;
            }
        }

        /// <summary>
        /// Helper method for serialization of this class."/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("headNode", _headNode);
            info.AddValue("levels", _levels);
            info.AddValue("comparer", _comparer);
        }
    }
}
