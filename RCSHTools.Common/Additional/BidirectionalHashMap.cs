using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RCSHTools
{
    public class BidirectionalHashMap<T1, T2> : IEnumerable<BidirectionalHashMap<T1, T2>.BiIndex>
    {
        public struct BiIndex
        {
            public T1 keyIndexer;
            public T2 valueIndexer;

            public BiIndex(T1 k, T2 v)
            {
                keyIndexer = k;
                valueIndexer = v;
            }
        }

        private List<BiIndex> indexes;
        private Dictionary<T1, int> keys;
        private Dictionary<T2, int> values;

        public T1 this[T2 t] => indexes[values[t]].keyIndexer;
        public T2 this[T1 t] => indexes[keys[t]].valueIndexer;

        public BidirectionalHashMap()
        {
            indexes = new List<BiIndex>();
            keys = new Dictionary<T1, int>();
            values = new Dictionary<T2, int>();
        }

        /// <summary>
        /// Adds an item to the map
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Append(T1 key, T2 value)
        {
            int index = indexes.Count;
            indexes.Add(new BiIndex(key, value));
            keys.Add(key, index);
            values.Add(value, index);
        }

        public IEnumerator<BiIndex> GetEnumerator()
        {
            return indexes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return indexes.GetEnumerator();
        }
    }
}
