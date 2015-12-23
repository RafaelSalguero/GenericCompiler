using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.CompilerStages.OperatorSolver;
using GenericCompiler.LexicalAnalysis.LexerUnits;
using System.Reflection;
namespace GenericCompiler.LexicalAnalysis
{
    /// <summary>
    /// Implements a lexer token preprocessor for common language tasks
    /// </summary>
    internal class LanguageDictionaryPreprocessor : IEnumerable<LexerPreprocessorDelegate>
    {
        private bool CaseSensitive;
        private IDictionary<string, Guid> Keywords;
        private bool RemoveWhiteSpaces;
        private bool RemoveLineJumps;

        public LanguageDictionaryPreprocessor(bool CaseSensitive, IDictionary<string, Guid> Keywords, bool RemoveWhiteSpaces, bool RemoveLineJumps)
        {
            this.CaseSensitive = CaseSensitive;
            this.Keywords = Keywords;
            this.RemoveWhiteSpaces = RemoveWhiteSpaces;
            this.RemoveLineJumps = RemoveLineJumps;
        }

        /// <summary>
        /// Identify keywords, remove whitespaces
        /// </summary>
        /// <returns></returns>
        private IEnumerable<LexerToken> SpaceKeyword(LexerToken Last, LexerToken Current, LexerToken Next)
        {
            if (Current.Token == Guid.Empty)
            {
                //Check if this token is a keyword:
                Guid Id;
                if (Keywords.TryGetValue(CaseSensitive ? Current.Substring() : Current.Substring().ToUpperInvariant(), out Id))
                    yield return new LexerToken(Current, Id);
                else
                    //Do not transform the current token:
                    yield return Current;
            }
            else if (Current.Token == SpecialTokens.WhiteSpace)
            {
                //Return only if remove whitespaces is false:
                if (!RemoveWhiteSpaces)
                    yield return Current;
            }
            else if (Current.Token == SpecialTokens.LineJump)
            {
                if (!RemoveWhiteSpaces)
                    yield return Current;
            }
            else
            {
                //Do not transform the current token:
                yield return Current;
            }

        }

        private IEnumerable<LexerPreprocessorDelegate> Preprocessor
        {
            get
            {
                yield return this.SpaceKeyword;
            }
        }

        public IEnumerator<LexerPreprocessorDelegate> GetEnumerator()
        {
            return Preprocessor.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    /// <summary>
    /// Handles definitions of keywords and operators, can be used as input for the Lexer and the LexerPreproccessor
    /// </summary>
    public class LanguageDictionary : CommonLexerTokens, ILexerAnalysisInput
    {
        private List<Operator> Operators = new List<Operator>();
        private Dictionary<string, Guid> Keywords = new Dictionary<string, Guid>();

        private bool CaseSensitive;
        private LanguageDictionaryPreprocessor Processor;
        /// <summary>
        /// Create a new language dictionary
        /// </summary>
        /// <param name="CaseSensitive">True if keywords are case sensitive</param>
        public LanguageDictionary(bool CaseSensitive)
        {
            this.CaseSensitive = CaseSensitive;
            this.Processor = new LanguageDictionaryPreprocessor(CaseSensitive, Keywords, true, true);
        }


        #region Preproccesors
        #endregion



        /// <summary>
        /// Add all the operators from an class that contains static Guid fields with the named guid attribute
        /// </summary>
        protected void AddKeywords<T>()
        {
            var Type = typeof(T);
            var Fields = Type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var F in Fields)
            {
                var NamedAtt = F.GetCustomAttributes<NamedGuidAttribute>().FirstOrDefault();
                if (NamedAtt != null && F.FieldType == typeof(Guid))
                {
                    AddKeyword(NamedAtt.Name, (Guid)F.GetValue(null));
                }
            }
        }

        /// <summary>
        /// Adds a keyword to the language dictionary
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        protected void AddKeyword(string Word, Guid Id)
        {
            if (!CaseSensitive)
                Word = Word.ToUpperInvariant();
            Keywords.Add(Word, Id);
            GuidNames.AddToken(Id, Word);
        }

        /// <summary>
        /// Adds a keyword to the language dictionary
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        protected Guid AddKeyword(string Word)
        {
            var Id = Guid.NewGuid();
            AddKeyword(Word, Id);
            return Id;
        }

        /// <summary>
        /// Add two special operators that will be used as code block begin and end markers. Their guids are in the SpecialTokens class
        /// </summary>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        protected void AddBlockSeparators(string Begin, string End)
        {
            Add(Begin, SpecialTokens.OpenBlock);
            Add(End, SpecialTokens.CloseBlock);
        }

        /// <summary>
        /// Add the Comma, LeftParenthesis, RightParenthesis and Dot operators to the dictionary
        /// </summary>
        /// <param name="AddCurlyBlocks">True to add curly braces as block separators</param>
        protected void AddSpecialOperators()
        {
            AddOperatorWord(",");
            AddOperatorWord("(");
            AddOperatorWord(")");
            AddOperatorWord(".");
        }


        /// <summary>
        /// Add an operator word and marks it as an operator, handle special operator cases comma, left and right parenthesis
        /// </summary>
        /// <param name="Op"></param>
        /// <returns></returns>
        protected Guid AddOperatorWord(string Op)
        {
            switch (Op)
            {
                case ",":
                    return Add(Op, SpecialTokens.Comma);
                case "(":
                    return Add(Op, SpecialTokens.LeftParenthesis);
                case ")":
                    return Add(Op, SpecialTokens.RightParenthesis);
                case ".":
                    return Add(Op, SpecialTokens.Dot);
                default:
                    return Add(Op);
            }
        }


        public IEnumerable<LexerPreprocessorDelegate> Preprocessor
        {
            get { return Processor; }
        }
    }
}
