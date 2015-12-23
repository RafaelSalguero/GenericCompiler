using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.BackusNaur.ParserResult
{
    /// <summary>
    /// Result of a sequence or repeated expression
    /// </summary>
    public class CollectionResult : PExpression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Items">Terminal results and empty collection results are ignored</param>
        /// <param name="ExpressionId"></param>
        public CollectionResult(IEnumerable<PExpression> Items, Guid ExpressionId)
            : base(ExpressionId)
        {
            this.Items = Items.Where((x) => !(
                //x is CollectionResult && !((CollectionResult)x).Items.Any() ||
                x is TerminalResult)).ToMemory();
        }

        public IEnumerable<PExpression> Items
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return "{" + Items.ToStringEnum(" ") + "}";
        }
    }
}
