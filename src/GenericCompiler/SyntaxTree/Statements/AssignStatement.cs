using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.SyntaxTree.Expressions;

namespace GenericCompiler.SyntaxTree.Statements
{
    /// <summary>
    /// A variable assign
    /// </summary>
    public class AssignStatement : Statement
    {
        /// <summary>
        /// Create a new assignment statement
        /// </summary>
        /// <param name="Field">The field to evaluate</param>
        /// <param name="Value">The value to assign</param>
        public AssignStatement(Expression Field, Expression Value)
            : base(ISubstringExtensions.BoundingConcat(Field, Value))
        {
            this.Field = Field;
            this.Value = Value;
        }

        public Expression Field
        {
            get;
            private set;
        }

        public Expression Value
        {
            get;
            private set;
        }
    }
}
