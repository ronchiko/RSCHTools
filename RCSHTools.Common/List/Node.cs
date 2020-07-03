namespace RCSHTools
{
    /// <summary>
    /// A node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T>
    {
        /// <summary>
        /// The next node
        /// </summary>
        public Node<T> Next { get; set; }
        /// <summary>
        /// The value of this node
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Creates a new Node
        /// </summary>
        /// <param name="value"></param>
        public Node(T value): this(value, null) { }
        /// <summary>
        /// Creates a new node
        /// </summary>
        /// <param name="value"></param>
        /// <param name="next"></param>
        public Node(T value, Node<T> next)
        {
            Value = value;
            Next = next;
        }
    }
}
