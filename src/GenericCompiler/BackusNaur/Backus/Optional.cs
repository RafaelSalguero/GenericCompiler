using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// Checks if a given token sequence match an expression, return a single result with a posible null value
    /// </summary>
    public class Optional : BackusExpression
    {
        public Optional(BackusExpression Expression)
        {
            this.Expression = Expression;
        }

        public BackusExpression Expression
        {
            get;
            private set;
        }

        protected override string InternalToString()
        {
            return "[" + Expression.ToString() + "]";
        }
    }
}
