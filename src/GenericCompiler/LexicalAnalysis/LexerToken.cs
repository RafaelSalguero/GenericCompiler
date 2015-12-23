using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.LexicalAnalysis
{
    public class LexerToken : TokenSubstring<Guid>, IEquatable<LexerToken>
    {
        public LexerToken(ISubstring Substring, Guid Token)
            : base(Substring, Token)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(LexerToken))
                return false;
            return Equals((LexerToken)obj);
        }

        public override int GetHashCode()
        {

            int variable = 10;
            Func<int> Func = () => { variable += 1; return variable; };

            if (Token == Guid.Empty)
                return (this.CharIndex * 2143 + 13543) ^ this.CharLen ^ this.CompleteString.GetHashCode();
            else
                return Token.GetHashCode();
        }

        public bool Equals(LexerToken other)
        {
            if (Token == Guid.Empty)
            {
                return this.ExactlyEqualsSubstring(other);
            }
            else
                return this.Token == other.Token;
        }

        public override string ToString()
        {
            if (Token == Guid.Empty)
                return this.Substring();
            else
                return "[" + GuidNames.GetName(Token) + "] " + this.Substring();
        }
    }
}
