using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// Match the longest posible repetition of a given expression. Empty sequences are valid.
    /// </summary>
    public class Repeated : BackusExpression
    {
        public Repeated(BackusExpression Expression)
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
            return "{" + Expression.ToString() + "}";
        }
    }
}
