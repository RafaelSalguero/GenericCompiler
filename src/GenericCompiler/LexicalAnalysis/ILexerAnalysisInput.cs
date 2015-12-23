using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.LexicalAnalysis.LexerUnits;

namespace GenericCompiler.LexicalAnalysis
{
    /// <summary>
    /// An object that contains a collection of ITokenized parsers
    /// </summary>
    public interface ILexerTokens
    {
        /// <summary>
        /// Gets the tokenized lexer units of this object
        /// </summary>
        IEnumerable<ITokenizedParser> Tokens { get; }
    }

    public interface ILexerPreprocessor
    {

        /// <summary>
        /// Gets the collection of preprocessing transformations of this preproceessor
        /// </summary>
        IEnumerable<LexerPreprocessorDelegate> Preprocessor { get; }
    }

    /// <summary>
    /// Contains tokens and preprocessing transformations, wich is the complete input for a lexical analysis pass
    /// </summary>
    public interface ILexerAnalysisInput : ILexerTokens, ILexerPreprocessor
    {

    }
}
