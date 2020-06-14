using System;
using System.Collections;
using System.Collections.Generic;

namespace RCSHTools
{
    /// <summary>
    /// How to read the binary tree
    /// </summary>
    public enum BinaryTreeReadMode
    {
        /// <summary>
        /// Reads Node->Node.left->Node.right
        /// </summary>
        Preorder,
        /// <summary>
        /// Reads Node.left->Node->Node.right
        /// </summary>
        Inorder,
        /// <summary>
        /// Reads Node.left->Node.right->Node
        /// </summary>
        Postorder,
    }

    internal class BinNode<T>
    {
        public T value;
        public BinNode<T> left;
        public BinNode<T> right;

        public BinNode(T value)
        {
            this.value = value;
        }
    }
    internal class BinNode : IComparable
    {
        public IComparable value;
        public BinNode left;
        public BinNode right;

        public BinNode(IComparable value) => this.value = value;

        public bool HasLeft() => left != null;
        public bool HasRight() => right != null;

        public int CompareTo(object obj) => value.CompareTo(obj);
    }

    /// <summary>
    /// Represents a non-generic binary search tree
    /// </summary>
    public class BinaryTree : ICollection<IComparable>{

        private BinNode head;

        /// <summary>
        /// Is the tree empty
        /// </summary>
        public bool IsEmpty => head == null;
        /// <summary>
        /// The amount of items in the tree
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// How to make the items in this tree linear
        /// </summary>
        public BinaryTreeReadMode ReadMode { get; set; }
        bool ICollection<IComparable>.IsReadOnly => false;

        /// <summary>
        /// Default comperison method for string
        /// </summary>
        public static readonly Comparison<string> CompareString = (string a, string b) => {

            int minLen = Math.Min(a.Length, b.Length);

            for (int i = 0; i < minLen; i++)
            {
                if(a[i] == b[i]) continue;
                return Math.Sign(a[i] - b[i]);
            }

            if(a.Length > b.Length) return 1;
            else if(b.Length > a.Length) return -1;

            return 0;
        } ;
        /// <summary>
        /// Creates a new empty binary tree
        /// </summary>
        public BinaryTree() : this(BinaryTreeReadMode.Inorder) { }
        /// <summary>
        /// Creates a new empty binary tree
        /// </summary>
        public BinaryTree(BinaryTreeReadMode readMode)
        {
            head = null;
            Count = 0;
            ReadMode = readMode;
        }

        /// <summary>
        /// Adds an item to the binary search tree
        /// </summary>
        /// <param name="item"></param>
        public void Add(IComparable item)
        {
            Count++;
            if(head == null)
            {
                head = new BinNode(item);
                return;
            }

            BinNode node = head;
            while(node != null)
            {
                int d = node.CompareTo(item);
                if(d > 0)
                {
                    if (node.HasRight()) node = node.right;
                    else
                    {
                        node.right = new BinNode(item);
                        return;
                    }
                }
                else
                {
                    if (node.HasLeft()) node = node.left;
                    else
                    {
                        node.left = new BinNode(item);
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// Removes an item from the tree
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IComparable item)
        {
            if (head == null) return false;

            Queue<BinNode> q = new Queue<BinNode>();

            // Push the rightmost path into a queue
            BinNode node = head;
            while(node != null)
            {
                q.Enqueue(node);
                node = node.right;
            }

            if(q.Count == 1) // Delete head
            {
                if (head.CompareTo(item) != 0) return false;
                head = null;
            }
            else
            {
                BinNode rightmost = q.Dequeue();
                BinNode parent = q.Dequeue();
                BinNode target = GetNode(item);

                if (target == null) return false;

                target.value = rightmost.value;
                parent.right = null;
            }
            Count--;
            return true;
        }
        /// <summary>
        /// Does the tree contains an item
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(IComparable other)
        {
            return GetNode(other) != null;
        }
        /// <summary>
        /// Clears the tree
        /// </summary>
        public void Clear()
        {
            head = null;
        }
        /// <summary>
        /// Copies th items in this tree into an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(IComparable[] array, int index)
        {
            foreach(IComparable node in this)
            {
                if (array.Length <= index) throw new IndexOutOfRangeException("Array too small");
                array[index] = node;
                index++;
            }
        }
        private BinNode GetNode(IComparable other)
        {
            BinNode node = head;
            while(node != null)
            {
                int d = node.CompareTo(other);
                if (d == 0) break;
                else if (d > 0) node = node.right;
                else node = node.left;
            }
            return node;
        }

        IEnumerator IEnumerable.GetEnumerator() => new BinaryTreeEnumerator(head, ReadMode);
        IEnumerator<IComparable> IEnumerable<IComparable>.GetEnumerator() => (IEnumerator<IComparable>)((IEnumerable)this).GetEnumerator();

        /// <summary>
        /// Enumerator for non-generic binary trees
        /// </summary>
        public class BinaryTreeEnumerator : IEnumerator
        {
            private Queue<BinNode> next, used;

            object IEnumerator.Current => next.Peek().value;
            /// <summary>
            /// The currently enumerated item
            /// </summary>
            public IComparable Current => next.Peek().value;

            internal BinaryTreeEnumerator(BinNode head, BinaryTreeReadMode mode)
            {
                next = new Queue<BinNode>();
                used = new Queue<BinNode>();

                switch (mode)
                {
                    case BinaryTreeReadMode.Preorder:
                        Pre(head);
                        break;
                    case BinaryTreeReadMode.Inorder:
                        In(head);
                        break;
                    case BinaryTreeReadMode.Postorder:
                        Post(head);
                        break;
                }
            }

            private void In(BinNode n)
            {
                In(n.left);
                next.Enqueue(n);
                In(n.right);
            }
            private void Pre(BinNode n)
            {
                next.Enqueue(n);
                Pre(n.left);
                Pre(n.right);
            }
            private void Post(BinNode n)
            {
                Post(n.left);
                Post(n.right);
                next.Enqueue(n);
            }

            bool IEnumerator.MoveNext()
            {
                used.Enqueue(next.Dequeue());
                return next.Count > 0;
            }
            void IEnumerator.Reset()
            {
                while (used.Count > 0)
                    next.Enqueue(used.Dequeue());
            }
        }
    }

    /// <summary>
    /// Generic binary search tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryTree<T> : ICollection<T>, IEnumerable{
        BinNode<T> head;
        Comparison<T> factor;

        /// <summary>
        /// The amount of nodes in the tree
        /// </summary>
        int ICollection<T>.Count
        {
            get
            {
                return Count(head);
            }
        }
        bool ICollection<T>.IsReadOnly => false;
        /// <summary>
        /// How to linearly read the binary tree
        /// </summary>
        public BinaryTreeReadMode ReadMode { get; set; } = BinaryTreeReadMode.Preorder;

        /// <summary>
        /// Constructs a binary tree with a custom comperison
        /// </summary>
        /// <param name="compare"></param>
        public BinaryTree(Comparison<T> compare) : this(compare, BinaryTreeReadMode.Preorder) { }
        /// <summary>
        /// Constructs a binary tree with a custom comperison
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="mode"></param>
        public BinaryTree(Comparison<T> compare, BinaryTreeReadMode mode){
            head = null;
            factor = compare;
            ReadMode = mode;
        }

        ///<summary>
        /// Adds an item to the seach tree
        ///</summary>
        public void Add(T value){
            if(head == null){
                head = new BinNode<T>(value);
                return;
            }

            BinNode<T> node = head;
            while(node != null)
            {
                int compare = factor(value, node.value);
                if (compare > 0)
                {
                    if(node.right == null)
                    {
                        node.right = new BinNode<T>(value);
                        return;
                    }
                    else
                    {
                        node = node.right;
                    }
                }
                else
                {
                    if (node.left == null)
                    {
                        node.left = new BinNode<T>(value);
                        return;
                    }
                    else
                    {
                        node = node.left;
                    }
                }
            }
        }

        ///<summary>
        /// Returns true if the value is part of the tree
        ///</summary>
        public bool HasItem(T value){
            if(head == null) return false;

            BinNode<T> root = head;

            while (root != null){
                int compare = factor(value, root.value);
            
                if(compare == 0) return true;
                
                root = compare > 0 ? root.right : root.left;
            }

            return false;
        }
        bool ICollection<T>.Contains(T item) => HasItem(item);

        ///<summary>
        /// Returns a list of all the of item in the tree 
        ///</summary>
        public List<T> Iteratable(){
            List<T> t = new List<T>();
            if(head != null)
            {
                t.Add(Iteratable(head, ref t).value);
            }
            return t;
        }
        private BinNode<T> Iteratable(BinNode<T> node, ref List<T> list){
            if(node == null) return null;

            if(node.left != null)
            {
                list.Add(Iteratable(node.left, ref list).value);
            }
            if(node.right != null){
                list.Add(Iteratable(node.right, ref list).value);
            }

            return node;
        }
        /// <summary>
        /// Clears the tree.
        /// </summary>
        public void Clear()
        {
            head = null;
        }

        ///<summary>
        /// Returns the amount of times an item is appeared int the tree
        ///</summary>
        public int Count(T value){
            return Count(head, value);
        }
        private int Count(BinNode<T> n, T value){
            if(n == null) return 0;
            return (factor(n.value, value) == 0 ? 1 : 0) + Count(n.left, value) + Count(n.right, value);
        }
        private int Count(BinNode<T> n)
        {
            if (n == null) return 0;
            return 1 + Count(n.left) + Count(n.right);
        }

        /// <summary>
        /// Deletes an item from the tree
        /// </summary>
        /// <param name="item"></param>
        public void Delete(T item){
            if (head == null)
                throw new Exception("Tree is empty");

            Queue<BinNode<T>> q = new Queue<BinNode<T>>();
            
            q.Enqueue(PushRightToQueue(head, ref q));

            if(q.Count == 1){
                if(factor(head.value,item) != 0)
                    throw new Exception("No item with value " + item);
                head = null;
            }else{
                BinNode<T> rightmost = q.Dequeue(), parent = q.Dequeue();
                BinNode<T> target = GetNode(head, item);

                if(target == null)
                    throw new Exception("No item with value " + item);

                target.value = rightmost.value;

                parent.right = null;
            }
        }
        /// <summary>
        /// Removes an item from the list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove(T item)
        {
            try
            {
                Delete(item);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private BinNode<T> GetNode(BinNode<T> n, T value){
            if(n == null) return null;

            if(factor(n.value, value) == 0) return n;

            if(n.left != null && factor(n.left.value, value) == 0) return n.left;
            if(n.right != null && factor(n.right.value, value) == 0) return n.right;

            BinNode<T> nx = GetNode(n.left, value);
            if(nx != null){
                return nx;
            }
            return GetNode(n.right, value);
        }
        private BinNode<T> PushRightToQueue(BinNode<T> n, ref Queue<BinNode<T>> t){
            if(n == null) return null;

            if(n.right != null)
                t.Enqueue(PushRightToQueue(n.right, ref t));

            return n;
        }


        ///<summary>
        /// Return true if the tree is perfect
        ///</summary>
        public bool IsPerfect(){
            int depth = FindDepth(head);

            return IsPerfect(head, depth, 0);
        }
        private bool IsPerfect(BinNode<T> n, int depth, int level){
            if(n == null) return true;

            if(n.right == null && n.left == null)
                return depth == level + 1;
            if(n.right == null || n.left == null)
                return false;

            return IsPerfect(n.left, depth, level + 1) && IsPerfect(n.right, depth, level + 1);
        }

        // Returns the depth of the leftmost node
        private int FindDepth(BinNode<T> n){
            int depth = 0;
            while(n != null){
                depth++;
                n = n.left;
            }
            return depth;
        }

        /// <summary>
        /// Copies the tree's content to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index)
        {
            if (head == null) return;

            Queue<BinNode<T>> queue = new Queue<BinNode<T>>();
            queue.Enqueue(head);
            
            while(queue.Count > 0)
            {
                if (index >= array.Length) throw new IndexOutOfRangeException("Array size is too small for this scale");
                BinNode<T> nt = queue.Dequeue();
                if (nt.left != null) queue.Enqueue(nt.left);
                if (nt.right != null) queue.Enqueue(nt.right);

                array[index] = nt.value;
                index++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BinaryTreeEnumrator(this, BinaryTreeReadMode.Inorder);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)((IEnumerable)this).GetEnumerator();
        }

        /// <summary>
        /// Emunerator for a binary tree
        /// </summary>
        public class BinaryTreeEnumrator : IEnumerator
        {
            private Queue<T> queue;
            private Queue<T> used;

            object IEnumerator.Current => queue.Peek();
            /// <summary>
            /// The currently pointed item
            /// </summary>
            public T Current => queue.Peek();

            /// <summary>
            /// Contructors for enumrator
            /// </summary>
            public BinaryTreeEnumrator(BinaryTree<T> tree, BinaryTreeReadMode mode)
            {
                queue = new Queue<T>();
                used = new Queue<T>();
                switch (mode)
                {
                    case BinaryTreeReadMode.Preorder:
                        Preorder(tree.head);
                        break;
                    case BinaryTreeReadMode.Inorder:
                        Inorderer(tree.head);
                        break;
                    case BinaryTreeReadMode.Postorder:
                        Postorder(tree.head);
                        break;
                    default:
                        throw new NotImplementedException("Not a supported read mode");
                }
            }

            private void Inorderer(BinNode<T> n)
            {
                Inorderer(n.left);
                queue.Enqueue(n.value);
                Inorderer(n.right);
            }
            private void Preorder(BinNode<T> n)
            {
                queue.Enqueue(n.value);
                Preorder(n.left);
                Preorder(n.right);
            }
            private void Postorder(BinNode<T> n)
            {
                Postorder(n.left);
                Postorder(n.right);
                queue.Enqueue(n.value);
            }

            bool IEnumerator.MoveNext()
            {
                used.Enqueue(queue.Dequeue());

                return queue.Count > 0;
            }
            void IEnumerator.Reset()
            {
                while (used.Count > 0)
                    queue.Enqueue(used.Dequeue());
            }
        }
    }
}