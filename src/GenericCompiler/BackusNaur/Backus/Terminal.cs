using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// Matches a given token 
    /// </summary>
    public class Terminal : BackusExpression
    {
        public Terminal(Guid Token)
            : this(Token, false)
        {
            //Set a name to this terminal token
            if (GuidNames.HasName(Token))
                GuidNames.AddToken(ExpressionId, GuidNames.GetName(Token));
        }

        protected Terminal(Guid Token, bool AsWord)
        {
            this.Token = Token;
            this.Word = AsWord;
        }


        /// <summary>
        /// If true the terminal will result in a WordResult instead of a TerminalResult, thus preserving the data
        /// </summary>
        internal bool Word
        {
            get;
            private set;
        }
        public Guid Token
        {
            get;
            private set;
        }

        protected override string InternalToString()
        {
            return GuidNames.GetName(Token);
        }
    }

    /// <summary>
    /// Matches a given token and preserves the written word
    /// </summary>
    public class Word : Terminal
    {
        public Word(Guid Token)
            : base(Token, true)
        {
        }

        public Word()
            : this(Guid.Empty)
        {

        }
    }
}
