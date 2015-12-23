using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.SyntaxTree.Statements
{
    /// <summary>
    /// A collection of statements
    /// </summary>
    public class BlockStatement : Statement
    {
        public BlockStatement(IEnumerable<Statement> Statements)
            : base(ISubstringExtensions.BoundingConcat(Statements))
        {
            this.Statements = Statements.ToList();
        }

        public IReadOnlyList<Statement> Statements
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return "{\r\n" + Statements.ToStringEnum("\r\n") + "\r\n}";
        }
    }
}
