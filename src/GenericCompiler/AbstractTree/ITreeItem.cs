using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.AbstractTree
{
    /// <summary>
    /// An value of type T wich can have children trees of the same type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T> : IEquatable<ITree<T>>
    {
        /// <summary>
        /// Item value
        /// </summary>
        T Value { get; set; }
        /// <summary>
        /// Tree subitems
        /// </summary>
        ITree<T>[] Subitems { get; }
    }
    [DebuggerDisplay("{ToString()}")]
    public class DummyTree<T> : ITree<T>
    {
        public DummyTree(T Value, ITree<T>[] Subitems)
        {

            this.Value = Value;
            this.subitems = Subitems;
        }
        public T Value
        {
            get;
            set;
        }

        ITree<T>[] subitems;
        public ITree<T>[] Subitems
        {
            get { return subitems; }
        }
        public override string ToString()
        {
            return this.TreeToString();
        }

        public bool Equals(ITree<T> other)
        {
            return TreeExtensions.TreeEquals(this, other);
        }
    }

    public static class TreeExtensions
    {
        public static ITree<T> CreateLeaf<T>(T Value)
        {
            return new DummyTree<T>(Value, null);
        }
        public static ITree<T> CreateSequence<T>(IEnumerable<T> Subitems, T Header = default(T))
        {
            return new DummyTree<T>(Header, Subitems.Select((x) => CreateLeaf(x)).ToArray());
        }
        public static ITree<T> CreateTree<T>(T Header, ITree<T>[] Subitems)
        {
            return new DummyTree<T>(Header, Subitems);
        }


        /// <summary>
        /// Convert a neasted sequence of ITree onto a tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Sequence"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ITree<T> AsTree<T>(this IEnumerable<IEnumerable<ITree<T>>> Sequence, T Value, T NeastedValue, bool OneIdentity)
        {
            if (OneIdentity)
                return new DummyTree<T>(Value, Sequence.Select
                    (
                        (x) =>
                        {
                            var arr = x.ToArray();
                            if (arr.Length == 1)
                                return arr[0];
                            else
                                return new DummyTree<T>(NeastedValue, x.ToArray());
                        }

                    ).ToArray());
            else
                return new DummyTree<T>(Value, Sequence.Select((x) => new DummyTree<T>(NeastedValue, x.ToArray())).ToArray());
        }

        /// <summary>
        /// Non-recursively apply a transformation to tree subitems and returns a new tree with the transformed subitems
        /// </summary>
        /// <returns></returns>
        public static ITree<T> TransformSequence<T>(this ITree<T> Tree, Func<IEnumerable<ITree<T>>, IEnumerable<ITree<T>>> Transform)
        {
            if (Tree.Subitems == null)
                return Tree;
            else
            {
                return new DummyTree<T>(Tree.Value, Transform(Tree.Subitems).ToArray());
            }
        }


        /// <summary>
        /// Compare tree value and all tree subitems recursively
        /// </summary>
        /// <returns></returns>
        public static bool TreeEquals<T>(ITree<T> A, ITree<T> B)
        {
            var Eq = EqualityComparer<T>.Default;
            if ((A.Value == null || B.Value == null) && !(A.Value == null && B.Value == null))
                return false;

            if (!Eq.Equals(A.Value, B.Value))
                return false;
            if (A.Subitems == null || B.Subitems == null)
                return A.Subitems == null && B.Subitems == null;
            else
                return A.Subitems.SequenceEqual(B.Subitems);
        }

        /// <summary>
        /// Returns true if item.Subitems == null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsLeaf<T>(this ITree<T> item)
        {
            return item.Subitems == null;
        }

        /// <summary>
        /// Returns true if item.Subitems == null and also if the tree value equals to the given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsLeaf<T>(this ITree<T> item, T LeafValue)
        {
            var Eq = EqualityComparer<T>.Default;
            return item.Subitems == null && Eq.Equals(item.Value, LeafValue);
        }



        /// <summary>
        /// Returns true if this item has an empty list of subitems
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsEmptySequence<T>(this ITree<T> item)
        {
            return item.Subitems != null && item.Subitems.Length == 0;
        }

        public static string TreeToString<T>(this ITree<T> item)
        {
            string valToString = item.Value == null ? "" : item.Value.ToString();
            if (item.IsLeaf())
                return valToString;
            else if (item.Subitems.Length == 0)
                return valToString + "()";
            else
                return valToString + "(" + item.Subitems.Select((x) => x.ToString()).Aggregate((a, b) => a + " " + b) + ")";
        }

    }
}
