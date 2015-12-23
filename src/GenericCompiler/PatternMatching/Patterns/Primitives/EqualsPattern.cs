using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.AbstractTree;

namespace GenericCompiler.PatternMatching.Patterns.Primitives
{
    /// <summary>
    /// Matches any token that equals the given value
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>}
    [DebuggerDisplay("{ToString}")]
    public class EqualsPattern<TKey, TValue> : IPattern<TKey, TValue>
    {
        public EqualsPattern(ITree<TValue> Value)
        {
            this.Value = Value;
        }
        public readonly ITree<TValue> Value;

        public IEnumerable<MatchResult<TKey, ITree<TValue>>> Match(ITree<TValue> Token)
        {
            if (Token.Equals(Value))
                yield return new MatchResult<TKey, ITree<TValue>>();
        }

        public override string ToString()
        {
            return "'" + Value.ToString() + "'";
        }
    }
}
