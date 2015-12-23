using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.SyntaxTree.Expressions
{
    /// <summary>
    /// An expression wich holds a literal value
    /// </summary>
    public class LiteralExpression : Expression
    {
        public LiteralExpression(object Value, ISubstring ParserPos)
            : base(ParserPos)
        {
            this.Value = Value;
        }
        public object Value
        {
            get;
            private set;
        }
    }
}
