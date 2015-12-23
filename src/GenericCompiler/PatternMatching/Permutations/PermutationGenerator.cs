using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.PatternMatching.Permutations
{
    /// <summary>
    /// Contains a collection of methods for performing lazy permutations and combinations
    /// </summary>
    public static class PermutationGenerator
    {
        /// <summary>
        /// Return a collection of permutations where the sum of every item
        /// in a list of K items equals to N, where every item is >= min. Order matters.
        ///
        /// example N = 4, K = 2, min = 1
        /// returns [1, 3], [2, 2], [3, 1]
        /// </summary>
        /// <param name="KItems">The number of items of every IntString</param>
        /// <param name="NSum">The sum of each IntString</param>
        /// <param name="minValue">The min value of each element of each IntString</param>
        /// <returns></returns>
        public static IEnumerable<IntString> GroupSizePermutation(int NSum, int KItems, int minValue)
        {
            if (KItems == 1)
            {
                IntString Ret = new IntString(1);
                Ret.items.Add(NSum);
                yield return Ret;
                yield break;
            }

            int maxValue = NSum - (KItems - 1) * minValue;
            if (maxValue < minValue) throw new ArgumentException();

            //Items exluding the last one, last is NSum - items sum
            int[] items = new int[KItems - 1];


            //The desired sum of K-1 items
            int desiredSum = NSum - minValue;

            //Reset items to minValue:
            for (int i = 0; i < items.Length; i++) items[i] = minValue;
            //The first sum value:
            int currentSum = minValue * items.Length;

            while (currentSum <= desiredSum)
            {
                yield return new IntString(items, NSum - currentSum);

                //Add one to the first item and update the current sum:
                items[0]++;
                currentSum++;

                for (int i = 1; i < items.Length; i++)
                {
                    if (currentSum > desiredSum)
                    {
                        //Reset the last item and update the current sum:
                        currentSum -= items[i - 1] - minValue;
                        items[i - 1] = minValue;

                        //Add one to this item:
                        items[i]++;
                        currentSum++;
                    }
                }
            }

        }

        /// <summary>
        /// Slice an IntString in sizes of every Sizes element
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="Sizes"></param>
        /// <returns></returns>
        public static void Slice(IntString Values, IntString Sizes, IntString[] result)
        {
            if (result.Length != Sizes.Count) throw new ArgumentException("result.Lenght != Sizes.count");

            int index = 0;
            for (int i = 0; i < Sizes.Count; i++)
            {
                int currentSize = Sizes[i];
                result[i] = new IntString(Values, index, currentSize);
                index += currentSize;
            }
        }

        private static void Swap<T>(T[] items, int a, int b)
        {
            T temp = items[a];
            items[a] = items[b];
            items[b] = temp;
        }

        /// <summary>
        /// Return all the ways to accomodate NItems in KBins where order doesn't matters
        /// </summary>
        /// <param name="NItems"></param>
        /// <param name="KBins"></param>
        /// <returns></returns>
        public static IEnumerable<IntString> Combinations(int NItems, int KBins)
        {
            int[] indices = new int[KBins];
            bool[] reset = new bool[KBins];
            //reset indices in inverted order:
            for (int i = 0; i < indices.Length; i++) indices[i] = i;

            yield return new IntString(indices);

            //indices = new int[] { 4, 3, 0 };

            while (true)
            {
                //Add one to the first index:
                indices[indices.Length - 1]++;

                if (indices[indices.Length - 1] < NItems)
                    yield return new IntString(indices);

                for (int i = 0; i < indices.Length; i++)
                {
                    int lastLimit = NItems - indices.Length + i;

                    reset[i] = (indices[i] >= lastLimit);
                }

                bool ret = false;

                if (reset[0])
                    yield break;

                for (int i = 0; i < indices.Length; i++)
                {
                    if (i < indices.Length - 1 && reset[i + 1])
                    {
                        indices[i]++;
                    }
                    if (reset[i])
                    {
                        indices[i] = indices[i - 1] + 1;
                        ret = true;
                    }
                }

                if (ret)
                    yield return new IntString(indices);

            }
        }

        /// <summary>
        /// Generate all the ways to accomodate NItems where order matters. Equivalent but faster than PartialPermutation(N, N)
        /// </summary>
        /// <param name="NItems"></param>
        /// <returns></returns>
        public static IEnumerable<IntString> Permutation(int NItems)
        {
            int[] choose = new int[NItems];
            for (int i = 0; i < NItems; i++) choose[i] = i;
            yield return new IntString(choose);

            int k;

            while (true)
            {
                k = -1;
                //Find the largest index such that a[k] < a[k + 1]
                for (int i = choose.Length - 2; i >= 0; i--)
                {
                    if (choose[i] < choose[i + 1])
                    {
                        k = i;
                        break;
                    }
                }
                if (k == -1)
                    yield break;

                //Find the largest index l such that a[k] < a[l]
                int l = -1;
                for (int i = choose.Length - 1; i >= 0; i--)
                {
                    if (choose[k] < choose[i])
                    {
                        l = i;
                        break;
                    }
                }

                //Swap k and l:
                int temp = choose[k];
                choose[k] = choose[l];
                choose[l] = temp;

                //Reverse from a[k+1] to a[n]
                Array.Reverse(choose, k + 1, choose.Length - k - 1);

                yield return new IntString(choose);
                if (k == -1) yield break;
            }
        }

        /// <summary>
        /// Generate all the ways to accomodate NItems in KBins where order matters
        /// </summary>
        /// <param name="NItems"></param>
        /// <param name="KBins"></param>
        /// <returns></returns>
        public static IEnumerable<IntString> PartialPermutation(int NItems, int KBins)
        {
            if (NItems < KBins) throw new ArgumentException("NItems < KBins");
            int i;
            int[] a = new int[NItems];
            for (i = 0; i < a.Length; i++) a[i] = i;

            yield return new IntString(a, 0, KBins);

            int edge = KBins - 1;

            // find j in (k…n-1) where aj > aedge

            while (true)
            {
                int j = KBins;

                while (j < NItems && a[edge] >= a[j])
                    ++j;

                if (j < NItems)
                {
                    //Swap aedge, aj
                    Swap(a, edge, j);
                }
                else
                {
                    if (a[0] == 4 && a[1] == 3 && a[2] == 2)
                        i = i;

                    Array.Reverse(a, KBins, a.Length - KBins);


                    i = edge - 1;
                    while (i >= 0 && a[i] >= a[i + 1])
                        --i;
                    if (i < 0)
                        break; ;


                    j = NItems - 1;
                    while (j > i && a[i] >= a[j])
                        --j;

                    Swap(a, i, j);
                    Array.Reverse(a, i + 1, a.Length - (i + 1));
                }

                yield return new IntString(a, 0, KBins);
            }
        }


        /// <summary>
        /// Return all the repeated combinations of a set of digits.
        /// 
        /// Example,
        /// if an array of two digits with the values [1, 2, 3] and [4, 5, 6] is used as input,
        /// the output is:
        /// [1, 4]
        /// [2, 4]
        /// [3, 4]
        /// 
        /// [1, 5]
        /// [2, 5]
        /// [3, 5]
        /// 
        /// [1, 6]
        /// [2, 6]
        /// [3, 6]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Digits"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> PowerCombine<T>(IEnumerable<T>[] Digits)
        {
            if (Digits.Length == 0) yield break;
            IEnumerator<T>[] Counters = new IEnumerator<T>[Digits.Length];
            T[] Current = new T[Digits.Length];

            //Set counters to the first element:
            for (int i = 0; i < Digits.Length; i++)
            {
                Counters[i] = Digits[i].GetEnumerator();
                if (!Counters[i].MoveNext())
                {
                    //If any element is empty, the result will be empty too
                    yield break;
                }
                Current[i] = Counters[i].Current;
            }
            Counters[0] = Digits[0].GetEnumerator();

            //Add one to the first counter:
            bool lastFinish = false;
            while (!lastFinish)
            {


                lastFinish = !Counters[0].MoveNext();
                if (!lastFinish)
                    Current[0] = Counters[0].Current;

                //Carry the one:
                for (int i = 1; i < Digits.Length; i++)
                {
                    if (lastFinish)
                    {
                        //Reset the last counter:
                        Counters[i - 1] = Digits[i - 1].GetEnumerator();
                        Counters[i - 1].MoveNext();
                        Current[i - 1] = Counters[i - 1].Current;

                        //Add one to the current:
                        lastFinish = !Counters[i].MoveNext();
                        if (!lastFinish)
                            Current[i] = Counters[i].Current;
                    }
                }

                if (!lastFinish)
                {
                    yield return Current;

                    Current = new T[Digits.Length];
                    for (int i = 0; i < Digits.Length; i++)
                        Current[i] = Counters[i].Current;
                }
            }


        }


    }
}
