using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.AbstractTree;

namespace GenericCompiler.PatternMatching.Patterns
{
    /// <summary>
    /// Pattern matching for abstract trees
    /// </summary>
    public interface IPattern<TKey, TLeaf> 
    {
        /// <summary>
        /// Returns a collection of posible matches of this pattern on a collection of tokens
        /// </summary>
        /// <param name="Tokens"></param>
        /// <returns></returns>
        IEnumerable<MatchResult<TKey, ITree<TLeaf>>> Match(ITree<TLeaf> Subject);
    }

}
