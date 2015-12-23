using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// A repeated expression separated by another expression, useful for parsing comma-separated expresions.
    /// The separator expression is not included as output when parsed
    /// </summary>
    public class Split : BackusExpression
    {
        public Split(BackusExpression Expression, BackusExpression Separator, bool AllowEmptySequences)
        {
            this.Expression = Expression;
            this.Separator = Separator;
            this.AllowEmptySequences = AllowEmptySequences;
        }

        public BackusExpression Expression
        {
            get;
            private set;
        }

        public BackusExpression Separator
        {
            get;
            private set;
        }

        public bool AllowEmptySequences
        {
            get;
            private set;
        }

        protected override string InternalToString()
        {
            return Expression.ToString() + Separator.ToString() + "...";
        }
    }
}
