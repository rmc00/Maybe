namespace Maybe.SkipList
{
    public class Node<T>
    {
        public Node<T>[] Next { get; }
        public T Value { get; }

        public Node(T value, int level)
        {
            Value = value;
            Next = new Node<T>[level];
        }

        public bool HasNextAtLevel(int level)
        {
            return Next[level] != null;
        }
    }
}
