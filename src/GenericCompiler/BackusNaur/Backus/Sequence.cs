using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// Match an ordered sequence of expressions
    /// </summary>
    public class Sequence : BackusExpression
    {
        public Sequence(IEnumerable<BackusExpression> Expressions)
        {
            this.Expressions = Expressions;
        }
        public Sequence(params BackusExpression[] Expressions)
            : this((IEnumerable<BackusExpression>)Expressions) { }

        public IEnumerable<BackusExpression> Expressions
        {
            get;
            private set;
        }

        protected override string InternalToString()
        {
            return Expressions.ToStringEnum(" ");
        }
    }
}
