using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.LexicalAnalysis;
using GenericCompiler.LexicalAnalysis.LexerUnits;
using GenericCompiler.PrecedenceParser;

namespace GenericCompiler.CompilerStages.OperatorSolver
{
    /// <summary>
    /// A default implementation of operator interfaces
    /// </summary>
    public class Operator : IArgPosOperator, IPrecedence, IIsParenthesis, IIsComma, IOriginalToken<string>, IToken<Guid>, IOperatorAssociativity, IArgCount
    {
        public Operator(OperatorArgumentPosition ArgumentPosition, int Precedence, string Name, Guid Token, OperatorAssociativity Associativity, int ArgumentCount)
        {
            this.ArgumentPosition = ArgumentPosition;
            this.Precedence = Precedence;
            this.OriginalToken = Name;
            this.Token = Token;
            this.Associativity = Associativity;
            this.ArgumentCount = ArgumentCount;
        }

        public OperatorArgumentPosition ArgumentPosition
        {
            get;
            private set;
        }

        public int Precedence
        {
            get;
            private set;
        }

        public bool IsClosedParenthesis
        {
            get
            {
                return Token == SpecialTokens.RightParenthesis;
            }
        }

        public bool IsOpenParenthesis
        {
            get { return Token == SpecialTokens.LeftParenthesis; }
        }

        public bool IsComma
        {
            get { return Token == SpecialTokens.Comma; }
        }

        public string OriginalToken
        {
            get;
            private set;
        }

        public Guid Token
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return OriginalToken;
        }

        public OperatorAssociativity Associativity
        {
            get;
            private set;
        }

        public int ArgumentCount
        {
            get;
            set;
        }
    }
}
