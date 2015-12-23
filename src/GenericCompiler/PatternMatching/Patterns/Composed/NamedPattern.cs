using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.PatternMatching.Patterns.Primitives;

namespace GenericCompiler.PatternMatching.Patterns
{
    /// <summary>
    /// Returns a named match of the subject token combined with the resulting matches of a given pattern
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class NamedPattern<TKey, TValue> : IPattern<TKey, TValue>
    {
        public NamedPattern(TKey Name, IPattern<TKey, TValue> Pattern)
        {
            this.Name = Name;
            this.Pattern = Pattern;
        }

        public TKey Name;
        public IPattern<TKey, TValue> Pattern;
        public IEnumerable<MatchResult<TKey, ITree<TValue>>> Match(ITree<TValue> Token)
        {
            var NameMatch = MatchResultFactory.Create(Name, Token);
            foreach (var match in Pattern.Match(Token))
            {
                yield return MatchResultFactory.JoinMatch(match, NameMatch);
            }
        }

        public override string ToString()
        {
            if (Pattern is AnyPattern<TKey, TValue>)
                return Name.ToString();
            else
                return "(" + Pattern + ")" + ":" + Name.ToString();
        }
    }
}
