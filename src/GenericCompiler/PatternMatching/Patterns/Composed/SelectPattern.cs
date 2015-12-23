using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.PatternMatching.Patterns.Composed
{
    /// <summary>
    /// Apply a given transform to the subject token before matching a given pattern
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class SelectPattern<TKey, TValue> : IPattern<TKey, TValue>
    {
        public SelectPattern(IPattern<TKey, TValue> Pattern, Func<ITree<TValue>, ITree<TValue>> Selector, string Description)
        {
            this.Pattern = Pattern;
            this.Selector = Selector;
            this.Description = Description;
        }
        public readonly IPattern<TKey, TValue> Pattern;
        public readonly Func<ITree<TValue>, ITree<TValue>> Selector;
        public readonly string Description;
        public IEnumerable<MatchResult<TKey, ITree<TValue>>> Match(ITree<TValue> Subject)
        {
            return Pattern.Match(Selector(Subject));
        }

        public override string ToString()
        {
            return Pattern.ToString() + "->" + Description;
        }
    }
}
