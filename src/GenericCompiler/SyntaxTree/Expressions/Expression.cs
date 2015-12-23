using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.SyntaxTree.Expressions
{
    /// <summary>
    /// Base class for all expression tree elements. All expressions are immutable. Expressions can't be statements.
    /// All expressions must result in a value when the expression is executed. Because of this, there are several things that can't be expressions:
    /// Assigments, If Then Else blocks, void method calls, code blocks, variable declarations, etc...
    /// </summary>
    public abstract class Expression : IOriginalToken<ISubstring>, ISubstring
    {
        public Expression(ISubstring OriginalToken)
        {
            this.OriginalToken = OriginalToken;
        }

        public ISubstring OriginalToken
        {
            get;
            private set;
        }

        string ISubstring.CompleteString
        {
            get { return OriginalToken.CompleteString; }
        }

        int ISubstring.CharIndex
        {
            get { return OriginalToken.CharIndex; }
        }

        int ISubstring.CharLen
        {
            get { return OriginalToken.CharLen; }
        }

        bool IEquatable<ISubstring>.Equals(ISubstring other)
        {
            return OriginalToken.Equals(other);
        }
    }
}
