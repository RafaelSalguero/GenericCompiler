using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// Maches one of a set of given expressions
    /// </summary>
    public class Cases : BackusExpression
    {
        public Cases(IEnumerable<BackusExpression> Expressions)
        {
            this.Expressions = Expressions;
        }
        public Cases(params BackusExpression[] Expressions)
            : this((IEnumerable<BackusExpression>)Expressions) { }

        public IEnumerable<BackusExpression> Expressions
        {
            get;
            private set;
        }

        protected override string InternalToString()
        {
            return "(" + Expressions.ToStringEnum(" | ") + ")";
        }
    }
}
