using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.CompilerStages.OperatorSolver
{


    /// <summary>
    /// Result from an operator solver 
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class DiscriminatedOperatorToken<TToken, TOperator> : IIsOperator, IOperator<TOperator>, IToken<TToken>, IIsParenthesis, IIsComma, ISubstring
        where TOperator : IIsParenthesis, IIsComma
        where TToken : ISubstring
    {
        public DiscriminatedOperatorToken(TToken Token, bool IsOperator, TOperator Operator)
        {
            this.Token = Token;
            this.IsOperator = IsOperator;
            this.Operator = Operator;
        }

        public TToken Token { get; private set; }

        /// <summary>
        /// Returns true if this is a solved operator
        /// </summary>
        public bool IsOperator
        { get; private set; }

        public TOperator Operator { get; private set; }

        public override string ToString()
        {
            if (IsOperator)
                return Token.ToString() + "[" + Operator.ToString() + "]";
            else
                return Token.ToString();
        }


        public bool IsClosedParenthesis
        {
            get { return IsOperator && Operator.IsClosedParenthesis; }
        }

        public bool IsOpenParenthesis
        {
            get { return IsOperator && Operator.IsOpenParenthesis; }
        }

        public bool IsComma
        {
            get { return IsOperator && Operator.IsComma; }
        }

        string ISubstring.CompleteString
        {
            get { return Token.CompleteString; }
        }

        int ISubstring.CharIndex
        {
            get { return Token.CharIndex; }
        }

        int ISubstring.CharLen
        {
            get { return Token.CharLen; }
        }

        bool IEquatable<ISubstring>.Equals(ISubstring other)
        {
            return Token.Equals(other);
        }
    }
}
