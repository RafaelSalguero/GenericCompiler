using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.PatternMatching.Patterns
{
    /// <summary>
    /// A pattern that checks for a list of patterns to match and returns the first pattern match that succeed
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class CasesPattern<TKey, TValue> : IPattern<TKey, TValue>
    {
        public CasesPattern(IEnumerable<IPattern<TKey, TValue>> Cases)
        {
            this.Cases = Cases;
        }


        /// <summary>
        /// Cases to check
        /// </summary>
        public readonly IEnumerable<IPattern<TKey, TValue>> Cases;

        public IEnumerable<MatchResult<TKey, AbstractTree.ITree<TValue>>> Match(AbstractTree.ITree<TValue> Subject)
        {
            foreach (var C in Cases)
            {
                bool any = false;
                foreach (var match in C.Match(Subject))
                {
                    yield return match;
                    any = true;
                }
                if (any) yield break;
            }
        }

        public override string ToString()
        {
            bool first = true;
            string s = "";
            foreach (var C in Cases)
            {
                if (!first)
                {
                    s += "|";
                }
                s += "(" + C.ToString() + ")";
                first = false;
            }
            return s + "";
        }
    }
}
