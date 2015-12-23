using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.SyntaxTree.Statements
{
    /// <summary>
    /// Statements are the actions that the program takes, are parallel to expressions. Statements doesn't return any values
    /// </summary>
    public class Statement : IOriginalToken<ISubstring>, ISubstring
    {
        public Statement(ISubstring OriginalToken)
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
