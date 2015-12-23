using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.ParserResult
{
    /// <summary>
    /// The result of parsing a terminal expression. This PExpression doesn't contain any values bounded with it
    /// </summary>
    /// <typeparam name="TTerminal"></typeparam>
    public class TerminalResult : PExpression
    {
        public TerminalResult(Guid ExpressionId)
            : base(ExpressionId)
        {
        }
        public override string ToString()
        {
            return GuidNames.GetName(ExpressionId);
        }
    }
}
