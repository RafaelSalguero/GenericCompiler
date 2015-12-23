using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.LexicalAnalysis.LexerUnits;
using GenericCompiler.LexicalAnalysis.LexerUnits.Parsers;
using System.Reflection;
namespace GenericCompiler.LexicalAnalysis
{
    /// <summary>
    /// Tokens with special meaning for the compiler engine. This token IDs should be used when representing the given token
    /// </summary>
    public static class SpecialTokens
    {
        /// <summary>
        /// A line jump (CR, CR_LF, or LF)
        /// </summary>
        public static Guid LineJump = Guid.NewGuid();

        /// <summary>
        /// A language specific line separator, such as the semicolon
        /// </summary>
        public static Guid CodeLineSeparator = Guid.NewGuid();

        /// <summary>
        /// A sequence of spaces and tabs
        /// </summary>
        public static Guid WhiteSpace = Guid.NewGuid();

        /// <summary>
        /// A decimal number
        /// </summary>
        public static Guid Number = Guid.NewGuid();

        /// <summary>
        /// A string literal
        /// </summary>
        public static Guid String = Guid.NewGuid();


        /// <summary>
        /// A function separator, commonly a comma
        /// </summary>
        public static Guid Comma = Guid.NewGuid();

        /// <summary>
        /// The function call operator, this is an internal operator
        /// </summary>
        internal static Guid Call = Guid.NewGuid();

        /// <summary>
        /// The dot operator
        /// </summary>
        public static Guid Dot = Guid.NewGuid();

        /// <summary>
        /// A left opening parenthesis
        /// </summary>
        public static Guid LeftParenthesis = Guid.NewGuid();

        /// <summary>
        /// A right closing parenthesis
        /// </summary>
        public static Guid RightParenthesis = Guid.NewGuid();


        /// <summary>
        /// When emmited from a lexer preprocessor, informs the begin of a new code block
        /// </summary>
        public static Guid OpenBlock = Guid.NewGuid();

        /// <summary>
        /// When emmited from a lexer preprocessor, informs the end of the last code block
        /// </summary>
        public static Guid CloseBlock = Guid.NewGuid();
    }

    /// <summary>
    /// Handle definitions of common language keywords and parser units as lexer tokens
    /// </summary>
    public abstract class CommonLexerTokens : ILexerTokens
    {
        private List<ITokenizedParser> Words = new List<ITokenizedParser>();



        protected void Add(ITokenizedParser TokenCreator)
        {
            this.Words.Add(TokenCreator);
        }
        /// <summary>
        /// Add a new word to the lexer dictionary and returns its Guid
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        protected Guid Add(string Word)
        {
            var Token = Guid.NewGuid();
            return Add(Word, Token);
        }


        /// <summary>
        /// Add a new word to the lexer dictionary and returns its Guid
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        protected Guid Add(string Word, Guid Token)
        {
            Add(new WordLexerUnit(Word).AsToken(Token));
            return Token;
        }



        /// <summary>
        /// Add all the operators from an class that contains static Guid fields with the named guid attribute
        /// </summary>
        protected void AddTokens<T>()
        {
            var Type = typeof(T);
            var Fields = Type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var F in Fields)
            {
                var NamedAtt = F.GetCustomAttributes<NamedGuidAttribute>().FirstOrDefault();
                if (NamedAtt != null && F.FieldType == typeof(Guid))
                {

                    Add(NamedAtt.Name, (Guid)F.GetValue(null));
                }
            }
        }


        protected Guid Add(Guid Token, INoLookupLexerUnitParser Parser)
        {
            Add(Parser.AsToken(Token));
            return Token;
        }

        protected Guid Add<T>(Guid Token) where T : INoLookupLexerUnitParser, new()
        {
            Add(Token, new T());
            return Token;
        }

        /// <summary>
        /// Add a LineJump lexer unit with the SpecialTokens.LineJump Id
        /// </summary>
        protected void AddLineJump()
        {
            Add<LineJumpLexerUnit>(SpecialTokens.LineJump);
        }

        /// <summary>
        /// Add a word as the code line separator using the SpecialTokens.CodeLineSeparator Id
        /// </summary>
        protected void AddCodeLineSeparator(string separator)
        {
            Add(separator, SpecialTokens.CodeLineSeparator);
        }

        /// <summary>
        /// Add a LineJump lexer unit with the SpecialToken.Number Id
        /// </summary>
        protected void AddNumber()
        {
            Add<NumberLexerUnit>(SpecialTokens.Number);
        }

        /// <summary>
        /// Add a LineJump lexer unit with the SpecialToken.WhiteSpace Id
        /// </summary>
        protected void AddWhiteSpace()
        {
            Add<WhiteSpaceLexerUnit>(SpecialTokens.WhiteSpace);
        }

        /// <summary>
        /// Add a LineJump unit with the SpecialToken.String Id
        /// </summary>
        protected void AddString()
        {
            Add<StringLexerUnit>(SpecialTokens.String);
        }



        public IEnumerable<ITokenizedParser> Tokens
        {
            get { return Words; }
        }
    }

}
