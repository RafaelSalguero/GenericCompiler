using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.PatternMatching
{
    internal static class MatchResultFactory
    {
        public static MatchResult<TKey, TValue> Create<TKey, TValue>(TKey Key, TValue Value)
        {
            var M = new MatchResult<TKey, TValue>();
            M.Values.Add(Key, Value);
            return M;
        }

        public static MatchResult<TKey, TValue> JoinMatch<TKey, TValue>(MatchResult<TKey, TValue> A, MatchResult<TKey, TValue> B)
        {
            Dictionary<TKey, TValue> Dic = A.Values;
            var Eq = EqualityComparer<TValue>.Default;

            foreach (var pair in B.Values)
            {
                TValue DicValue;
                if (Dic.TryGetValue(pair.Key, out DicValue))
                {
                    if (!Eq.Equals(DicValue, pair.Value))
                        return null;
                }
                else
                    Dic.Add(pair.Key, pair.Value);
            }
            return new MatchResult<TKey, TValue>(Dic);
        }

        /// <summary>
        /// Returns a match containing all matches key-values if congruent, otherwise returns null
        /// </summary>
        /// <returns></returns>
        public static MatchResult<TKey, TValue> JoinMatch<TKey, TValue>(IEnumerable<MatchResult<TKey, TValue>> Matches)
        {
            Dictionary<TKey, TValue> Dic = new Dictionary<TKey, TValue>();
            var Eq = EqualityComparer<TValue>.Default;

            foreach (var match in Matches)
            {
                foreach (var pair in match.Values)
                {
                    TValue DicValue;
                    if (Dic.TryGetValue(pair.Key, out DicValue))
                    {
                        if (!Eq.Equals(DicValue, pair.Value))
                            return null;
                    }
                    else
                        Dic.Add(pair.Key, pair.Value);
                }
            }
            return new MatchResult<TKey, TValue>(Dic);
        }

    }

    /// <summary>
    /// A dictionary of key-token of a resulting pattern match.
    /// An instance of a match result with empty values represent a succeed match
    /// An empty collection of matches represent a failed match
    /// </summary>
    /// <typeparam name="TKey">A key of the matched value dictionary</typeparam>
    /// <typeparam name="TToken">The type of matched values</typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class MatchResult<TKey, TValue>
    {
        public MatchResult()
            : this(new Dictionary<TKey, TValue>())
        {
        }
        public MatchResult(Dictionary<TKey, TValue> Values)
        {
            this.Values = Values;
        }

        /// <summary>
        /// Create a match succeed result without any value
        /// </summary>
        public Dictionary<TKey, TValue> Values;

        public override string ToString()
        {
            string r = "";
            foreach (var key in Values)
            {
                r += key.Key.ToString() + " = " + key.Value.ToString() + "; ";
            }
            return r;
        }


    }
}
