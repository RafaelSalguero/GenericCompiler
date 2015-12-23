using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //Always return a single empty match result, wich is a pass
            yield return new MatchResult<TKey, ITree<TValue>>();
        }

        public override string ToString()
        {
            return "%";
        }
    }
}
