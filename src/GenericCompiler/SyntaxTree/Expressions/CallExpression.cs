using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.SyntaxTree.Expressions
{
    /// <summary>
    /// A function call, the function is represented by an expression and the arguments as subexpressions
    /// </summary>
    public class CallExpression : GroupExpression
    {
        public CallExpression(Expression Function, IEnumerable<Expression> Arguments)
            : base(Arguments)
        {
            this.Function = Function;
        }

        public Expression Function
        {
            get;
            private set;
        }


        public override string ToString()
        {
            return Function.ToString() + "(" + this.Subexpressions.ToStringEnum(", ") + ")";
        }
    }
}
