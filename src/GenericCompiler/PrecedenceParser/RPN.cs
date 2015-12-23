using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.CompilerStages.OperatorSolver;

namespace GenericCompiler.PrecedenceParser
{
    public interface IIsFunction
    {
        bool IsFunction { get; }
    }

    public enum OperatorAssociativity
    {
        None,
        Left,
        Right
    }
    public interface IOperatorAssociativity
    {
        OperatorAssociativity Associativity { get; }
    }

    public static class RPN
    {
        private enum StackTokenType
        {
            None,
            Value,
            Operand,
            Function,
            Comma,
            LeftPar,
            RightPar,
            OpenCommaGroup,
            CloseCommaGroup
        }

        public static IEnumerable<StackToken<TToken, TOperator>> ToRPN<TToken, TOperator>(IEnumerable<DiscriminatedOperatorToken<TToken, TOperator>> Tokens)
            where TOperator : IPrecedence, IIsParenthesis, IIsComma, IOperatorAssociativity
            where TToken : ISubstring
        {
            var Ret = ToRPN<DiscriminatedOperatorToken<TToken, TOperator>, TToken, TOperator>(Tokens);
            foreach (var OR in Ret)
            {
                var R = OR.OriginalToken;
                if (OR.IsFunction)
                    yield return StackToken<TToken, TOperator>.CreateFunction(R.Token, OR.ArgumentCount);
                else if (OR.IsOperator)
                    yield return StackToken<TToken, TOperator>.CreateOperator(R.Operator);
                else
                    yield return StackToken<TToken, TOperator>.CreateValue(R.Token);
            }
        }

        private static IEnumerable<RpnToken<T>> ToRPN<T, TToken, TOperator>(IEnumerable<T> Tokens)
            where T : IIsOperator, IToken<TToken>, IOperator<TOperator>
            where TOperator : IPrecedence, IIsParenthesis, IIsComma, IOperatorAssociativity
        {
            RpnToken<T>[] tokens = ToRpnTokens<T, TToken, TOperator>(Tokens);
            var Ret = InfixToRPN(tokens);
            return Ret;
        }


        private interface ISetArgCount
        {
            int ArgumentCount { set; }
        }

        private class RpnToken<T> : IPrecedence, IIsOperator, IIsParenthesis, IIsComma, IIsFunction, IOperatorAssociativity, IOriginalToken<T>, ISetArgCount, IArgCount
        {
            public RpnToken(bool IsOperator, int Precedence, bool IsClosedParenthesis, bool IsOpenParenthesis, bool IsComma, bool IsFunction, OperatorAssociativity Associativity, T OriginalToken)
            {
                this.IsOperator = IsOperator;
                this.Precedence = Precedence;
                this.IsClosedParenthesis = IsClosedParenthesis;
                this.IsOpenParenthesis = IsOpenParenthesis;
                this.IsComma = IsComma;
                this.IsFunction = IsFunction;
                this.OriginalToken = OriginalToken;
                this.Associativity = Associativity;
            }

            public bool IsOperator
            {
                get;
                set;
            }
            public T OriginalToken
            { get; set; }


            public int Precedence
            {
                get;
                private set;
            }

            public bool IsClosedParenthesis
            {
                get;
                private set;
            }

            public bool IsOpenParenthesis
            {
                get;
                private set;
            }

            public bool IsComma
            {
                get;
                private set;
            }

            public bool IsFunction
            {
                get;
                private set;
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


        private static bool PopOperatorByPrecedence<T>(T To, T SkPeek) where T : IPrecedence, IOperatorAssociativity
        {
            if (To.Associativity == OperatorAssociativity.Left)
                return To.Precedence <= SkPeek.Precedence;
            else if (To.Associativity == OperatorAssociativity.Right)
                return To.Precedence < SkPeek.Precedence;
            else
                throw new NotSupportedException("Associativity of operators must be left or right");
        }


        private static RpnToken<T>[] ToRpnTokens<T, TToken, TOperator>(IEnumerable<T> Tokens)
            where T : IIsOperator, IToken<TToken>, IOperator<TOperator>
            where TOperator : IPrecedence, IIsParenthesis, IIsComma, IOperatorAssociativity
        {
            var Items = Tokens.ToArray();
            var Ret = new RpnToken<T>[Items.Length];

            for (int i = 0; i < Items.Length; i++)
            {
                var It = Items[i];
                if (It.IsOperator)
                {
                    var Op = It.Operator;
                    Ret[i] = new RpnToken<T>(true, Op.Precedence, Op.IsClosedParenthesis, Op.IsOpenParenthesis, Op.IsComma, false, Op.Associativity, It);
                }
                else
                {
                    var isFunction = i < Items.Length - 1 && Items[i + 1].IsOperator && Items[i + 1].Operator.IsOpenParenthesis;
                    Ret[i] = new RpnToken<T>(false, 0, false, false, false, isFunction, OperatorAssociativity.None, It);
                }
            }
            return Ret;
        }

        private static StackTokenType GetType<T>(T To)
         where T : IPrecedence, IIsParenthesis, IIsComma, IIsOperator, IIsFunction, IOperatorAssociativity, ISetArgCount
        {

            StackTokenType StackType;
            if (To.IsFunction)
                StackType = StackTokenType.Function;
            else if (To.IsOpenParenthesis)
                StackType = StackTokenType.LeftPar;
            else if (To.IsClosedParenthesis)
                StackType = StackTokenType.RightPar;
            else if (To.IsComma)
                StackType = StackTokenType.Comma;
            else if (To.IsOperator)
                StackType = StackTokenType.Operand;
            else
                StackType = StackTokenType.Value;
            return StackType;
        }

        /// <summary>
        /// Convert infix to RPN notation
        /// </summary>
        /// <returns></returns>
        private static List<T> InfixToRPN<T>(T[] Items) where
            T : IPrecedence, IIsParenthesis, IIsComma, IIsOperator, IIsFunction, IOperatorAssociativity, ISetArgCount
        {
            Queue<T> Out = new Queue<T>();
            Stack<T> Sk = new Stack<T>();


            for (int i = 0; i < Items.Length; i++)
            {
                var To = Items[i];

                StackTokenType StackType = GetType(To);

                switch (StackType)
                {
                    case StackTokenType.Value:
                        Out.Enqueue(To);

                        break;
                    case StackTokenType.Operand:
                        while (
                            (Sk.Count > 0) &&
                            (GetType(Sk.Peek()) == StackTokenType.Operand) && (PopOperatorByPrecedence(To, Sk.Peek())))
                            Out.Enqueue(Sk.Pop());
                        Sk.Push(To);
                        break;
                    case StackTokenType.Function:
                        Out.Enqueue(To);
                        Sk.Push(To);


                        break;
                    case StackTokenType.Comma:
                        {
                            while (!Sk.Peek().IsOpenParenthesis)
                                Out.Enqueue(Sk.Pop());
                        }
                        break;
                    case StackTokenType.LeftPar:
                        Sk.Push(To);
                        break;
                    case StackTokenType.RightPar:
                        {
                            while (Sk.Count > 0 && !Sk.Peek().IsOpenParenthesis)
                                Out.Enqueue(Sk.Pop());
                            if (Sk.Count == 0)
                            {
                                throw new ArgumentException("Wrong parenthesis balance");
                            }
                            Sk.Pop();
                            if (Sk.Count > 0 && Sk.Peek().IsFunction)
                            {
                                var f = Sk.Pop();
                                Out.Enqueue(f);
                            }
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            while (Sk.Count > 0)
            {
                Out.Enqueue(Sk.Pop());
            }

            List<T> Ret = new List<T>();
            while (Out.Count > 0)
                Ret.Add(Out.Dequeue());

            //Check for mismatched parenthesis:
            foreach (var Token in Ret)
            {
                if (Token.IsOpenParenthesis || Token.IsClosedParenthesis)
                    throw new ArgumentException("Mismatched parenthesis");
            }

            return Ret;
        }

    }

}
