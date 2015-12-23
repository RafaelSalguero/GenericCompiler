using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.CompilerStages.MultiGroping;
using GenericCompiler.TextSide;
using EnumerableExtensions;
namespace GenericCompiler.LexicalParser
{
    public static class CommentParser
    {
        /// <summary>
        /// A collection of ITokenSubstring of bool where the token is true the output token is a comment
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="CommentEnclosers"></param>
        /// <param name="ExclusiveEnclosers"></param>
        /// <returns></returns>
        public static IEnumerable<ITokenSubstring<bool>> ParseComments(
            ISubstring Code,
            IEnumerable<Tuple<string, string>> CommentEnclosers,
            IEnumerable<string> ExclusiveEnclosers
            )
        {
            var Separators = CommentEnclosers.SelectMany((x) => new string[] { x.Item1, x.Item2 });
            var Words = Code.SplitWords(Separators);


            var Groupping = TokenGrouping.CreateFlatGroup(CommentEnclosers, ExclusiveEnclosers);
            var Groups =
                TokenGrouping.GroupTokens(Groupping, Words, (x) => x.Substring())
                .AggregateAdjacents
                (
                    (x, y) => x.IsLeaf && y.IsLeaf,
                    (a, b) => new GroupToken<ISubstring, string>(a.LeafValue.Concat(b.LeafValue))
                )
                .Select
                (
                    (x) => x.IsLeaf ?
                    x.LeafValue.AsToken(false) :
                    x.Subitems.Select((y) => y.LeafValue).Aggregate((a, b) => a.Concat(b)).AsToken(true)
                );

            return Groups;
        }
    }
}

