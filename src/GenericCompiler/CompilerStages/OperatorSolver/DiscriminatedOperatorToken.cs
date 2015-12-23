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
    public class DiscriminatedOperatorToken<TToken, TOperator>
    {
        public DiscriminatedOperatorToken(TToken Token, bool IsOperator, TOperator Operator)
        {
            this.Token = Token;
            this.IsOperator = IsOperator;
            this.Operator = Operator;
        }

        public TToken Token;

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
    }
}
