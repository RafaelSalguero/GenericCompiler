using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.CompilerStages.OperatorSolver;

namespace GenericCompiler.PrecedenceParser
{
    public interface IArgCount
    {
        int ArgumentCount { get; }
    }

    /// <summary>
    /// Stack tokens can be variable names, values, operators or functions.
    /// Function and operators contain its argument count
    /// The function is defined by its token, while the operator is defined by the operator property
    /// </summary>
    public class StackToken<TToken, TOperator> : IIsFunction, IIsOperator, IOperator<TOperator>, IToken<TToken>, IArgCount
    {
        public StackToken(bool IsFunction, bool IsOperator, TOperator Operator, TToken Token, int ArgumentCount)
        {
            this.IsFunction = IsFunction;
            this.IsOperator = IsOperator;
            this.Operator = Operator;
            this.Token = Token;
            this.ArgumentCount = ArgumentCount;
        }

        public static StackToken<TToken, TOperator> CreateValue(TToken Value)
        {
            return new StackToken<TToken, TOperator>(false, false, default(TOperator), Value, 0);
        }
        public static StackToken<TToken, TOperator> CreateOperator(TOperator Operator)
        {
            return new StackToken<TToken, TOperator>(false, true, Operator, default(TToken), 0);
        }
        public static StackToken<TToken, TOperator> CreateFunction(TToken FunctionName, int ArgumentCount)
        {
            return new StackToken<TToken, TOperator>(true, false, default(TOperator), FunctionName, ArgumentCount);
        }


        public bool IsFunction
        {
            get;
            private set;
        }

        public bool IsOperator
        {
            get;
            private set;
        }

        public TOperator Operator
        {
            get;
            private set;
        }

        public TToken Token
        {
            get;
            private set;
        }

        public int ArgumentCount
        {
            get;
            private set;
        }

        public override string ToString()
        {
            if (IsOperator)
                return Operator.ToString();
            else if (IsFunction)
                return Token.ToString() + "[" + ArgumentCount.ToString() + "]";
            else
                return Token.ToString();
        }
    }
}
