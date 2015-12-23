using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.LexicalAnalysis.LexerUnits
{
    /// <summary>
    /// A tokenized word lenght
    /// </summary>
    public struct LexerWordLenght
    {
        /// <summary>
        /// Create a new valid token lexer word
        /// </summary>
        /// <param name="Lenght"></param>
        /// <param name="Token"></param>
        public LexerWordLenght(int Lenght, Guid Token)
        {
            this.Lenght = Lenght;
            this.Token = Token;
        }

        /// <summary>
        /// Create a new non-token word
        /// </summary>
        /// <param name="Lenght"></param>
        public LexerWordLenght(int Lenght)
            : this(Lenght, Guid.Empty)
        {

        }

        /// <summary>
        /// Gets the lenght of the lexer word
        /// </summary>
        public readonly int Lenght;

        /// <summary>
        /// Gets the ID of the lexer unit that generated this lexer word
        /// </summary>
        public readonly Guid Token;

        /// <summary>
        /// Returns weather this lexer word is a valid token
        /// </summary>
        public bool IsToken
        {
            get
            {
                return Token != Guid.Empty;
            }
        }

        public override string ToString()
        {
            if (IsToken)
                return Lenght.ToString() + " [" + GuidNames.GetName(Token) + "]";
            else
                return Lenght.ToString();
        }
    }


}
