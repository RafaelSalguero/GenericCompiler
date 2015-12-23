using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.LexicalAnalysis.LexerUnits.Parsers;
using EnumerableExtensions;
using GenericCompiler.LexicalAnalysis.LexerUnits;
namespace GenericCompiler.LexicalAnalysis
{
    public delegate IEnumerable<LexerToken> LexerPreprocessorDelegate(LexerToken Last, LexerToken Current, LexerToken Next);


    /// <summary>
    /// Implements a lexer unit separator, tokenization and preprocessing as a single step
    /// </summary>
    public static class Lexer
    {
        /// <summary>
        /// Tokenize and preprocess a given text
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<LexerToken> LexicalAnalysis(this ILexerAnalysisInput Input, ISubstring Text)
        {
            var Tokens = Input.Tokens.Tokenize(Text);
            var PP = Input.Preprocessor.Preprocess(Tokens);
            return PP;
        }

        /// <summary>
        /// Apply a series of preprocess transformations to a collection of lexer tokens
        /// </summary>
        /// <param name="Tokens"></param>
        /// <param name="Preprocessor"></param>
        /// <returns></returns>
        public static IEnumerable<LexerToken> Preprocess(this IEnumerable<LexerPreprocessorDelegate> Preprocessor, IEnumerable<LexerToken> Tokens)
        {
            IEnumerable<LexerToken> Current = Tokens;
            foreach (var P in Preprocessor)
            {
                Current = P.Preprocess(Current);
            }
            return Current;
        }

        /// <summary>
        /// Apply a preprocessor function to a collection of lexer tokens
        /// </summary>
        /// <param name="Tokens"></param>
        /// <param name="Preprocessor"></param>
        /// <returns></returns>
        public static IEnumerable<LexerToken> Preprocess(this LexerPreprocessorDelegate Preprocessor, IEnumerable<LexerToken> Tokens)
        {
            return Tokens.SelectMany(new EnumerableExtensions.SelectManyLookupDelegate<LexerToken, LexerToken>
                ((last, current, next, i, pos) => Preprocessor(last, current, next)));
        }

        /// <summary>
        /// Split words and returns an enumeration of TokenSubstring of Guid
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LexerToken> Tokenize(this IEnumerable<ITokenizedParser> Units, ISubstring Text)
        {
            return Tokenize(Text, SplitWordLenghts(Text, Units));
        }

        public static IEnumerable<LexerToken> Tokenize(ISubstring Text, IEnumerable<LexerWordLenght> WordLens)
        {
            int currentCharIndex = Text.CharIndex;
            foreach (var Len in WordLens)
            {
                var Substring = new Substring(Text.CompleteString, currentCharIndex, Len.Lenght);
                currentCharIndex += Len.Lenght;

                var TokenSS = new LexerToken(Substring, Len.Token);
                yield return TokenSS;
            }
        }

        public static List<LexerWordLenght> SplitWordLenghts(string Text, params ITokenizedParser[] Units)
        {
            return SplitWordLenghts(new Substring(Text), Units);
        }
        /// <summary>
        /// Split words and returns all lens and tokens of words
        /// </summary>
        /// <returns></returns>
        public static List<LexerWordLenght> SplitWordLenghts(ISubstring Text, params ITokenizedParser[] Units)
        {
            return SplitWordLenghts(Text, (IEnumerable<ITokenizedParser>)Units);
        }

        /// <summary>
        /// Split words and returns all lens and tokens of words
        /// </summary>
        /// <returns></returns>
        public static List<LexerWordLenght> SplitWordLenghts(ISubstring Text, IEnumerable<ITokenizedParser> Units)
        {
            var Separator = new SubstringLexerSeparator(Units);  //new LexerUnitSeparator(Units);
            return Separator.Split(Text);
        }


    }
}
