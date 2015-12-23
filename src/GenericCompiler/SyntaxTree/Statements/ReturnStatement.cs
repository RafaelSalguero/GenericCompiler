using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.SyntaxTree.Expressions;

namespace GenericCompiler.SyntaxTree.Statements
{
    /// <summary>
    /// A method return statement
    /// </summary>
    public class ReturnStatement : Statement
    {
        public ReturnStatement(Expression Value)
            : base(Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// The expression to return
        /// </summary>
        public Expression Value { get; private set; }
    }
}
