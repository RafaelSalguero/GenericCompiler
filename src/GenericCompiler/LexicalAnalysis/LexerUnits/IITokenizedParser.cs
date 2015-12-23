using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.LexicalAnalysis.LexerUnits.Parsers;

namespace GenericCompiler.LexicalAnalysis.LexerUnits
{
    public interface ITokenizedParser : INoLookupLexerUnitParser
    {
        Guid Token { get; }
    }

    /// <summary>
    /// A parser constructor paired with an immutable token id
    /// </summary>
    public class SingleTokenParser : ITokenizedParser
    {
        public SingleTokenParser(Guid Token, INoLookupLexerUnitParser Parser)
        {
            GuidNames.AddToken(Token, Parser.ToString());
            this.Token = Token;
            this.Parser = Parser;
        }

        private INoLookupLexerUnitParser Parser;

        public Guid Token
        {
            get;
            private set;
        }

        void INoLookupLexerUnitParser.Append(char Current)
        {
            Parser.Append(Current);
        }

        void INoLookupLexerUnitParser.Reset()
        {
            Parser.Reset();
        }

        LexerUnitValidity INoLookupLexerUnitParser.CurrentValidity
        {
            get
            {
                return Parser.CurrentValidity;
            }
        }

        public override string ToString()
        {
            return Parser.ToString() + " " + Parser.CurrentValidity.ToString();
        }
    }
}
