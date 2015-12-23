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
    /// Apply a test on the subject token, returns the match of the given pattern if the test succeeds, if the test fails,
    /// the pattern isn't matched againsts the subject and returns a failed match
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TLeaf"></typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class TestPattern<TKey, TLeaf> : IPattern<TKey, TLeaf>
    {
        /// <summary>
        /// Create a new where pattern
        /// </summary>
        /// <param name="Predicate">Returns true if the token pass the predicate</param>
        public TestPattern(IPattern<TKey, TLeaf> Pattern, Func<ITree<TLeaf>, bool> Predicate, string Description)
        {
            this.Pattern = Pattern;
            this.Predicate = Predicate;
            this.Description = Description;
        }
        public readonly IPattern<TKey, TLeaf> Pattern;
        public readonly Func<ITree<TLeaf>, bool> Predicate;
        public readonly string Description;
        public IEnumerable<MatchResult<TKey, ITree<TLeaf>>> Match(ITree<TLeaf> Token)
        {
            if (Predicate(Token))
                return Pattern.Match(Token);
            else
            {

                return new MatchResult<TKey, ITree<TLeaf>>[0];
            }
        }

        public override string ToString()
        {
            if (Pattern is AnyPattern<TKey, TLeaf>)
                return Description;
            else
                return "(" + Pattern.ToString() + ")?" + Description;
        }
    }
}

