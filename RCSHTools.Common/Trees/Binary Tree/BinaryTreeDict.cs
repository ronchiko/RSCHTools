using System;
using System.Collections.Generic;

namespace RCSHTools
{
    internal class Node<T1, T2>
    {
        public T1 id;
        public T2 value;

        public Node<T1, T2> left, right;

        public Node(T1 id, T2 value)
        {
            this.id = id;
            this.value = value;
        }
    }

    internal class NodeOperation<T1, T2>
    {
        public Node<T1, T2> node;
        public char op;

        private int helper;

        public NodeOperation(Node<T1,T2> node, char op)
        {
            helper = 0;
            this.node = node;
            this.op = op;
        }

        /// <summary>
        /// Runs an operation on a node, returns true if the operation was finished, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool Operate(Node<T1, T2> n)
        {
            switch (op)
            {
                case 'b':
                    if (helper == 0)
                        node.left = n;
                    else
                        node.right = n;
                    helper++;
                    return helper == 2;
                case 'r':
                    node.right = n;
                    return true;
                case 'l':
                    node.left = n;
                    return true;
                default:
                    return true;
            }
        }
    }

    public class BinaryTree<TKey, TValue> {

        private Node<TKey, TValue> head;
        private Comparison<TKey> compare;

        public BinaryTree(Comparison<TKey> compare){
            this.compare = compare;
            head = null;
        }

        #region Cache
        internal static BinaryTree<string, string> LoadFromCache(string[] lines)
        {
            BinaryTree<string, string> cache = new BinaryTree<string, string>((string x, string y) => {
                int len = Math.Min(x.Length, y.Length);
                for (int i = 0; i < len; i++)
                {
                    if (x[i] > y[i]) return 1;
                    else if (x[i] < y[i]) return -1;
                }

                if (x.Length > y.Length) return -1;
                else if (x.Length < y.Length) return 1;

                return 0;
            });

            if (lines.Length == 0) return cache;

            int andIndex = lines[0].IndexOf('&');
            Queue<NodeOperation<string, string>> operations = new Queue<NodeOperation<string, string>>(); 
             
            cache.head = new Node<string, string>(lines[0].Substring(0, andIndex), lines[0].Substring(andIndex + 1,lines[0].Length - andIndex - 2));

            operations.Enqueue(new NodeOperation<string, string>(cache.head, lines[0][lines[0].Length - 1]));

            if (lines.Length < 2) return cache;

            int linePointer = 1;
            while(operations.Count > 0)
            {
                NodeOperation<string, string> operation = operations.Peek();

                andIndex = lines[linePointer].IndexOf('&');
                Node<string, string> node = new Node<string, string>(lines[linePointer].Substring(0, andIndex), lines[linePointer].Substring(andIndex + 1, lines[linePointer].Length - andIndex - 2));

                char opmode = lines[linePointer][lines[linePointer].Length - 1];
                if(opmode != 'n')
                    operations.Enqueue(new NodeOperation<string, string>(node,opmode));

                bool finished = operation.Operate(node);

                if (finished)
                    operations.Dequeue();

                linePointer++;
            }

            return cache;
        }
        
        private static char GetOperationMode(Node<TKey,TValue> node)
        {
            if (node.right != null && node.left != null)
                return 'b';
            else if (node.right != null) return 'r';
            else if (node.left != null) return 'l';
            return 'n';
        }

        public void CreateCacheFile(string path, bool createNew)
        {
            if (head == null) throw new Exception("Nothing to cache");

            Queue<Node<TKey, TValue>> nodes = new Queue<Node<TKey, TValue>>();

            nodes.Enqueue(head);

            while (nodes.Count > 0)
            {
                Node<TKey, TValue> node = nodes.Dequeue();

                if (node == null) continue;

                nodes.Enqueue(node.left);
                nodes.Enqueue(node.right);

                System.IO.File.AppendAllText(path,node.id + "&" + node.value + GetOperationMode(node) + "\n");
            }
        }
        #endregion
        
        // Indexer
        public TValue this[TKey key] => Get(key);

        /// <summary>
        /// Adds an item to the tree
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        public void Add(TKey identifier,TValue value)
        {
            if(head == null)
            {
                head = new Node<TKey, TValue>(identifier, value);
                return;
            }

            Node<TKey, TValue> node = head;
            while(node != null)
            {
                int c = compare(identifier, node.id);
                if (c > 0)
                {
                    if (node.right == null)
                    {
                        node.right = new Node<TKey, TValue>(identifier,value);
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
                        node.left = new Node<TKey, TValue>(identifier, value);
                        return;
                    }
                    else
                    {
                        node = node.left;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets an item for the tree
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            if(head == null)
            {
                return default(TValue);
            }

            Node<TKey, TValue> node = head;
            while(node != null)
            {
                if (node.id.Equals(key))
                {
                    return node.value;
                }

                int c = compare(key, node.id);

                if (c == 0) return node.value;
                node = c > 0 ? node.right : node.left;
            }
            return default(TValue);
        }

        ///<summary>
        /// Returns a list of all the items in the tree
        ///</summary>
        public Dictionary<TKey, TValue> Iteratable(){
            Dictionary<TKey, TValue> a = new Dictionary<TKey, TValue>();
            Node<TKey, TValue> n = Iteratable(head, ref a);
            if(n != null)
                a.Add(n.id, n.value);
            return a;
        }
        private Node<TKey,TValue> Iteratable(Node<TKey,TValue> node, ref Dictionary<TKey, TValue> append){
            if(node == null) return null;           

            if(node.left != null){
                Node<TKey, TValue> left = Iteratable(node.left, ref append);
                append.Add(left.id, left.value);
            }
            if(node.right != null){
                Node<TKey, TValue> right = Iteratable(node.right, ref append);
                append.Add(right.id, right.value);
            }

            return node;
        }

        ///<summary>
        /// Returns the amount of times an item with a key appears in the tree
        ///</summary>
        public int Count(TKey id){
            return Count(head, id);
        }
        private int Count(Node<TKey, TValue> n, TKey v){
            if(n == null) return 0;

            return (compare(n.id, v) == 0? 1 : 0) + Count(n.left, v) + Count(n.right, v); 
        }

        public void Delete(TKey item){
            Queue<Node<TKey,TValue>> q = new Queue<Node<TKey, TValue>>();

            if(head == null)
                throw new Exception("Tree does not have a head");

            q.Enqueue(PushToQueue(head, ref q));

            if(q.Count == 1){
                if(compare(head.id, item) != 0)
                    throw new Exception("No item with key " + item);
                head = null;
            }
            else
            {
                Node<TKey, TValue> rightmost = q.Dequeue(), parent = q.Dequeue();
                Node<TKey, TValue> target = GetNode(head ,item);

                if(target == null)
                    throw new Exception("No item with key " + item);

                target.id = rightmost.id;
                target.value = rightmost.value;

                parent.right = null;
            }
        }
        private Node<TKey,TValue> PushToQueue(Node<TKey,TValue> n, ref Queue<Node<TKey, TValue>> q){
            if(n == null) return null;

            if(n.right != null)
                q.Enqueue(PushToQueue(n.right, ref q));

            return n;
        }   
        private Node<TKey, TValue> GetNode(Node<TKey, TValue> n, TKey value){
            if(n == null) return null;

            if(compare(n.id, value) == 0) return n;

            if(n.left != null && compare(n.left.id, value) == 0) return n.left;
            if(n.right != null && compare(n.right.id, value) == 0) return n.right;

            Node<TKey, TValue> nx = GetNode(n.left, value);
            if(nx != null){
                return nx;
            }
            return GetNode(n.right, value);
        }

        ///<summary>
        /// Returns true if the tree is a perfect one
        ///</summary>
        public bool IsPerfect(){
            int depth = FindDepth(head);
            return IsPerfect(head, depth, 0);
        }

        private bool IsPerfect(Node<TKey, TValue> node, int depth, int level){
            if(node == null)
                return true;
            if(node.left == null && node.right == null)
                return depth == level + 1;
            if(node.left == null || node.right == null)
                return false;
            
            return IsPerfect(node.right, depth, level + 1) && IsPerfect(node.left, depth, level + 1);
        }

        private int FindDepth(Node<TKey, TValue> n){
            int depth = 0;
            while(n != null){
                depth ++;
                n = n.left;
            }
            return depth;
        }
    
        public List<TValue> Select(TKey id){
            List<TValue> a = new List<TValue>();
            Node<TKey, TValue> n = Select(head, id, ref a);
            if(n != null)
                a.Add(n.value);
            return a;
        }
        private Node<TKey,TValue> Select(Node<TKey,TValue> node, TKey id, ref List<TValue> append){
            if(node == null) return null;           

            if(node.left != null){
                Node<TKey, TValue> left = Select(node.left, id, ref append);
                if(compare(left.id,id) == 0) append.Add(left.value);
            }
            if(node.right != null){
                Node<TKey, TValue> right = Select(node.right, id, ref append);
                if(compare(right.id,id) == 0) append.Add(right.value);
            }

            return node;
        }
    }
}