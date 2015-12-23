using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.SyntaxTree.Expressions
{
    public class GroupExpression : Expression
    {
        /// <summary>
        /// An expression that can contain other subexpressions as arguments
        /// </summary>
        public GroupExpression(IEnumerable<Expression> Subexpressions)
            : base(ISubstringExtensions.BoundingConcat(Subexpressions))
        {
            this.Subexpressions = new ReadOnlyCollection<Expression>(Subexpressions.ToList());
        }

        public ReadOnlyCollection<Expression> Subexpressions
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Subexpressions.ToStringEnum(", ");
        }
    }

}
