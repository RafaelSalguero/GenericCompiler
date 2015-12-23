using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.PatternMatching.Permutations
{
    /// <summary>
    /// A string of integers
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct IntString : IEnumerable<int>, IEquatable<IntString>
    {
        public IntString(int capacity)
        {
            items = new List<int>(capacity);
        }
        public IntString(string digits)
        {
            items = new List<int>(digits.Length);
            for (int i = 0; i < digits.Length; i++)
            {
                items.Add(int.Parse(digits.Substring(i, 1)));
            }
        }

        public IntString(int[] items, int index, int count)
        {
            this.items = new List<int>(count);
            for (int i = index; i < index + count; i++)
                this.items.Add(items[i]);
        }
        public IntString(IntString items, int index, int count)
        {
            this.items = new List<int>(count);
            for (int i = index; i < index + count; i++)
                this.items.Add(items[i]);
        }

        public IntString(IEnumerable<int> items)
        {
            this.items = new List<int>(items);
        }
        public IntString(IEnumerable<int> items, int lastItem)
        {
            this.items = new List<int>(items);
            this.items.Add(lastItem);
        }



        public List<int> items;

        public int this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }

        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }
        public int Sum
        {
            get
            {
                int ret = 0;
                foreach (var i in items)
                    ret += i;
                return ret;
            }
        }

        public static IntString operator +(IntString A, IntString B)
        {
            IntString Ret = new IntString(A.Count + B.Count);
            Ret.items.AddRange(A.items);
            Ret.items.AddRange(B.items);
            return Ret;
        }

        /// <summary>
        /// Map a collection of indices onto values
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="Indices"></param>
        /// <returns></returns>
        public static IntString Map(IntString Values, IntString Indices)
        {
            IntString Ret = new IntString(Indices.Count);
            foreach (var i in Indices)
                Ret.items.Add(Values.items[i]);
            return Ret;
        }



        public override string ToString()
        {
            string S = "";
            foreach (var i in items)
                S += i.ToString() + " ";
            return S;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }


        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var S in items)
            {
                hash += S;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            return hash;
        }

        public bool Equals(IntString other)
        {
            if (this.Count != other.Count)
                return false;
            for (int i = 0; i < this.Count; i++)
                if (this.items[i] != other.items[i])
                    return false;
            return true;
        }
    }
}
