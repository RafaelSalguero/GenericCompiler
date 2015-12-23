using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.AbstractTree;

namespace GenericCompiler.PatternMatching.Patterns.Composed
{
    /// <summary>
    /// Test a given pattern on all elements of a sequence and if all of them succeed, returns
    /// a match join of all matching results. If the subject isn't a sequence, returns the pattern applied to that token.
    /// Doesn't match empty sequences
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class AllPattern<TKey, TValue> : IPattern<TKey, TValue>
    {
        public AllPattern(IPattern<TKey, TValue> Pattern)
        {
            this.Pattern = Pattern;
        }

        public readonly IPattern<TKey, TValue> Pattern;
        public IEnumerable<MatchResult<TKey, ITree<TValue>>> Match(ITree<TValue> Subject)
        {
            if (Subject.IsLeaf())
                return Pattern.Match(Subject);
            else
            {
                if (Subject.Subitems.Length == 0)
                    return new MatchResult<TKey, ITree<TValue>>[0];
                else if (Subject.Subitems.Length == 1)
                {
                    return Pattern.Match(Subject.Subitems[0]);
                }
                else
                {
                    var MatchDigits = new MatchResult<TKey, ITree<TValue>>[Subject.Subitems.Length][];
                    for (int i = 0; i < Subject.Subitems.Length; i++)
                    {
                        MatchDigits[i] = Pattern.Match(Subject.Subitems[i]).ToArray();

                        //If any subject fails to match, the power combine would return an empty result, so we end the search early:
                        if (MatchDigits[i].Length == 0)
                            return new MatchResult<TKey, ITree<TValue>>[0];
                    }

                    var Power = Permutations.PermutationGenerator.PowerCombine(MatchDigits);
                    return Power.Select((x) => MatchResultFactory.JoinMatch(x)).Where((x) => x != null);
                }
            }

        }

        public override string ToString()
        {
            return Pattern.ToString() + "..";
        }
    }
}
