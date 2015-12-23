using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.LexicalAnalysis.LexerUnits.Parsers
{
    /// <summary>
    /// Parse a lexer unit that has to match an specified word. Every word has its own guid, word lexer units created
    /// with the same word share the same Id
    /// </summary>
    public class WordLexerUnit : INoLookupLexerUnitParser
    {
        public WordLexerUnit(string Word)
        {
            this.Word = Word;
            Reset();
        }


        private readonly string Word;
        /// <summary>
        /// The last valid lenght
        /// </summary>
        private int ValidLen = 0;

        public void Reset()
        {
            ValidLen = 0;
            if (Word.Length == 0)
                Validity = LexerUnitValidity.Valid;
            else
                Validity = LexerUnitValidity.Posible;
        }

        public void Append(char Current)
        {
            if (ValidLen < Word.Length && Word[ValidLen] == Current)
            {
                ValidLen++;
                Validity = ValidLen < Word.Length ? LexerUnitValidity.Posible : LexerUnitValidity.Valid; ;
            }
            else
                Validity = LexerUnitValidity.Invalid;
        }

        private LexerUnitValidity Validity;
        public LexerUnitValidity CurrentValidity
        {
            get { return Validity; }
        }

        public override string ToString()
        {
            return Word;
        }
    }
}
