using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.PatternMatching.Permutations;
using EnumerableExtensions;
namespace GenericCompiler.PatternMatching.Patterns
{
    /// <summary>
    /// Match a collection of ordered patterns onto a tree subitems 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TLeaf"></typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class SequencePattern<TKey, TLeaf> : IPattern<TKey, TLeaf>
    {
        public SequencePattern
            (
            TLeaf Header,
            IPattern<TKey, TLeaf>[] Sequence,
            bool TrySingleSequences = false,
            bool OneIdentity = true,
            bool TryEmptySequences = true,
            bool TryZeroDefault = false,
            ITree<TLeaf> ZeroDefault = default (ITree<TLeaf >)
            )
        {
            this.Header = Header;
            this.Sequence = Sequence;
            this.TrySingleSequences = TrySingleSequences;
            this.TryOneIdentity = OneIdentity;
            this.TryEmptySequences = TryEmptySequences;

            this.TryZeroDefault = TryZeroDefault;
            this.ZeroDefault = ZeroDefault;
        }

        /// <summary>
        /// Only match trees that have this header
        /// </summary>
        public readonly TLeaf Header;

        /// <summary>
        /// True to try single element sequences on matching
        /// </summary>
        public readonly bool TrySingleSequences;
        /// <summary>
        /// True if a single element sequence of this element equals to the same element
        /// </summary>
        public readonly bool TryOneIdentity;

        /// <summary>
        /// True to try zero element sequences on matching
        /// </summary>
        public readonly bool TryEmptySequences;

        /// <summary>
        /// True to try the default element when an empty sequence is tested
        /// </summary>
        public readonly bool TryZeroDefault;
        /// <summary>
        /// Values for zero-element sequences
        /// </summary>
        public readonly ITree<TLeaf> ZeroDefault;


        /// <summary>
        /// Sequence pattern to match
        /// </summary>
        public readonly IPattern<TKey, TLeaf>[] Sequence;

        public IEnumerable<MatchResult<TKey, ITree<TLeaf>>> Match(ITree<TLeaf> Token)
        {
            if (Token.IsLeaf())
                yield break;
            var LeafEq = EqualityComparer<TLeaf>.Default;
            if (!LeafEq.Equals(Token.Value, Header))
                yield break;

            var Subitems = Token.Subitems;
            var GroupSizes = PermutationGenerator.GroupSizePermutation(Subitems.Length, Sequence.Length, 0);
            foreach (var Size in GroupSizes)
            {
                var Digits = new IEnumerable<MatchResult<TKey, ITree<TLeaf>>>[Sequence.Length];
                int sliceStart = 0;
                for (int i = 0; i < Size.Count; i++)
                {
                    int sliceSize = Size[i];
                    var sliceItems = Subitems.Subsegment(sliceStart, sliceSize);

                    if (sliceSize == 1)
                    {
                        List<MatchResult<TKey, ITree<TLeaf>>> DigitValue = new List<MatchResult<TKey, ITree<TLeaf>>>();
                        if (TrySingleSequences)
                        {
                            var sliceToken = TreeExtensions.CreateTree(Header, sliceItems);
                            DigitValue.AddRange(Sequence[i].Match(sliceToken));
                        }
                        if (TryOneIdentity)
                        {
                            DigitValue.AddRange(Sequence[i].Match(sliceItems[0]));
                        }
                        Digits[i] = DigitValue;
                    }
                    else if (sliceSize == 0)
                    {
                        List<MatchResult<TKey, ITree<TLeaf>>> DigitValue = new List<MatchResult<TKey, ITree<TLeaf>>>();
                        if (TryEmptySequences)
                        {
                            var sliceToken = TreeExtensions.CreateTree(Header, sliceItems);
                            DigitValue.AddRange(Sequence[i].Match(sliceToken));
                        }
                        if (TryZeroDefault)
                        {
                            DigitValue.AddRange(Sequence[i].Match(ZeroDefault));
                        }
                        Digits[i] = DigitValue;
                    }
                    else
                    {
                        var sliceToken = TreeExtensions.CreateTree(Header, sliceItems);
                        Digits[i] = Sequence[i].Match(sliceToken);
                    }

                    sliceStart += sliceSize;
                }
                var Power = PermutationGenerator.PowerCombine(Digits);
                foreach (var String in Power)
                {
                    var Join = MatchResultFactory.JoinMatch(String);
                    if (Join != null)
                        yield return Join;
                }
                //var Join = MatchResultFactory.JoinMatch(Power);
            }
        }

        public override string ToString()
        {
            bool first = true;
            string s = "";
            foreach (var C in Sequence)
            {
                if (!first)
                {
                    s += " ";
                }
                s += C.ToString();
                first = false;
            }
            return s + "";
        }
    }
}
