using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.PatternMatching.Patterns.Composed;
using GenericCompiler.PatternMatching.Patterns.Primitives;
using EnumerableExtensions;
namespace GenericCompiler.PatternMatching.Patterns
{
    public static class PatternFactory
    {
        /// <summary>
        /// A pattern that matches any non-empty pattern
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Any<TKey, TLeaf>()
        {
            return new AnyPattern<TKey, TLeaf>();
        }

        /// <summary>
        /// A pattern that matches any non leaf pattern
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> NonLeaf<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern, bool AllowEmptySequence = false)
        {
            if (AllowEmptySequence)
                return Pattern.Test((x) => !x.IsLeaf(), "[..]");
            else
                return Pattern.Test((x) => !x.IsLeaf() && !x.IsEmptySequence(), "[_,..]");
        }

        /// <summary>
        /// A pattern that matches any leaf, tree, or non-empty sequence
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> NonEmpty<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern)
        {
            return Pattern.Test((x) => !x.IsEmptySequence(), "![]");
        }



        /// <summary>
        /// A pattern that matches any leaf
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TLeaf"></typeparam>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Leaf<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern)
        {
            return Pattern.Test((x) => x.IsLeaf(), "_");
        }
        /// <summary>
        /// Matches any leaf
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TLeaf"></typeparam>
        /// <returns></returns>
        public static IPattern<TKey,TLeaf> Leaf<TKey,TLeaf>()
        {
            return Any<TKey, TLeaf>().Leaf();
        }

        /// <summary>
        /// A pattern that matches an empty sequence
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TLeaf"></typeparam>
        /// <param name="Literal"></param>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Empty<TKey, TLeaf>()
        {
            return Any<TKey, TLeaf>().Empty();
        }

        /// <summary>
        /// A pattern that matches an empty sequence
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TLeaf"></typeparam>
        /// <param name="Literal"></param>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Empty<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern)
        {
            return Pattern.Test((x) => x.IsEmptySequence(), "[]");
        }

        /// <summary>
        /// A pattern that matches if the subject token is exactly equal to the given literal
        /// </summary>
        /// <param name="Leaf"></param>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Literal<TKey, TLeaf>(ITree<TLeaf> Literal)
        {
            return new EqualsPattern<TKey, TLeaf>(Literal);
        }

        /// <summary>
        /// A pattern that matches if the subject token is exactly equal to the given literal
        /// </summary>
        /// <param name="Leaf"></param>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Literal<TKey, TLeaf>(TLeaf LeafLiteral)
        {
            return Literal<TKey, TLeaf>(TreeExtensions.CreateLeaf(LeafLiteral));
        }



        /// <summary>
        /// A pattern that first checks if the subject token is an empty sequence and returns an empty succeed match,
        /// else check for the given pattern
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> AsOptional<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern)
        {
            return new OptionalPattern<TKey, TLeaf>(Pattern);
        }

        public static IPattern<TKey, TLeaf> Named<TKey, TLeaf>(TKey Name)
        {
            return Any<TKey, TLeaf>().AsNamed(Name);
        }

        /// <summary>
        /// A pattern that assign the subject value onto the given name if the given pattern matches
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static NamedPattern<TKey, TLeaf> AsNamed<TKey, TLeaf>(this  IPattern<TKey, TLeaf> Pattern, TKey Name)
        {
            return new NamedPattern<TKey, TLeaf>(Name, Pattern);
        }


        /// <summary>
        /// Convert an enumeration of patterns onto a sequence pattern
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TLeaf"></typeparam>
        /// <param name="Patterns"></param>
        /// <param name="Header"></param>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> AsSequencePattern<TKey, TLeaf>(
            this IEnumerable<IPattern<TKey, TLeaf>> Patterns,
                TLeaf Header = default(TLeaf )
            )
        {
            return new SequencePattern<TKey, TLeaf>(Header, Patterns.ToArray());
        }

        /// <summary>
        /// Apply a selector to the subject token before this pattern performs matching
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Select<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern, Func<ITree<TLeaf>, ITree<TLeaf>> Selector, string Description = "")
        {
            return new SelectPattern<TKey, TLeaf>(Pattern, Selector, Description);
        }

        /// <summary>
        /// Apply a selector that splits the subject token before matching
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Split<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern, TLeaf Separator, bool OneIdentity = true)
        {
            return Select<TKey, TLeaf>(Pattern, (x) =>
            {
                if (x.IsLeaf()) return x;
                else
                    return x.Subitems.Split((y) => y.IsLeaf(Separator)).AsTree(x.Value, x.Value, OneIdentity);
            }, "split");
        }

        /// <summary>
        /// Test the given pattern on all elements of a subject sequence and returns a match join of all matches.
        /// If one or more elements fail the match, the result of All is also a failed match.
        /// Doesn't match empty sequences
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> All<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern)
        {
            return new AllPattern<TKey, TLeaf>(Pattern);
        }


        /// <summary>
        /// Match the given pattern only if first the test passes for the subject token
        /// </summary>
        /// <returns></returns>
        public static IPattern<TKey, TLeaf> Test<TKey, TLeaf>(this IPattern<TKey, TLeaf> Pattern, Func<ITree<TLeaf>, bool> Test, string TestDescription)
        {
            return new TestPattern<TKey, TLeaf>(Pattern, Test, TestDescription);
        }
    }
}
