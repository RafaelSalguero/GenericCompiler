using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.ParserResult
{
    /// <summary>
    /// The result of parsing a Word expression, contains the parsed string word 
    /// </summary>
    public class WordResult : PExpression
    {
        public WordResult(string Word, Guid ExpressionId)
            : base(ExpressionId)
        {
            this.Word = Word;
        }

        public string Word
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Word;
        }
    }
}
