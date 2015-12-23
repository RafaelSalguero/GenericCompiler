using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.SyntaxTree.Expressions
{
    /// <summary>
    /// A primitive expression contains a guid primtive token and other sub expressions.
    /// Primitive expression can be used to represent operators
    /// </summary>
    public class PrimitiveExpression : GroupExpression
    {
        public PrimitiveExpression(Guid Token, IEnumerable<Expression> Arguments)
            : base(Arguments)
        {

        }

        /// <summary>
        /// The primitive token of this expression
        /// </summary>
        public Guid Token
        {
            get;
            private set;
        }
    }
}
