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
    /// Matches any non-empty token
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>}
    [DebuggerDisplay("{ToString}")]
    public class AnyPattern<TKey, TValue> : IPattern<TKey, TValue>
    {
        public IEnumerable<MatchResult<TKey, ITree<TValue>>> Match(ITree<TValue> Token)
        {
            yield return new MatchResult<TKey, ITree<TValue>>();
        }

        public override string ToString()
        {
            return "%";
        }
    }
}
