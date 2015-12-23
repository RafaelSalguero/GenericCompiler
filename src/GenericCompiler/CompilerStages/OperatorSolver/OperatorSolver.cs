using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.CompilerStages.OperatorSolver
{
    public static class OperatorSolver
    {
        private static DiscriminatedOperatorToken<TToken, TOperator> CreateDOT<TToken, TOperator>(TToken Token, TOperator Operator)
            where TOperator : IIsComma, IIsParenthesis
            where TToken : ISubstring 
        { return new DiscriminatedOperatorToken<TToken, TOperator>(Token, true, Operator); }

        private static DiscriminatedOperatorToken<TToken, TOperator> CreateDOT<TToken, TOperator>(TToken Token)
             where TOperator : IIsComma, IIsParenthesis
            where TToken : ISubstring 

        { return new DiscriminatedOperatorToken<TToken, TOperator>(Token, false, default(TOperator)); }



        public static bool IsRightArgument(int i,
                    Solving[] Tokens)
        {
            if (i >= Tokens.Length)
                return false;

            switch (Tokens[i])
            {
                case Solving.Value:
                case Solving.OpenParenthesis:
                case Solving.Prefix:
                    return true;
                case Solving.Binary:
                case Solving.Postfix:
                case Solving.PostfixOrBinary:
                case Solving.ClosedParenthesis:
                case Solving.Comma:
                    return false;

                case Solving.PrefixOrBinary:
                    return IsRightArgument(i + 1, Tokens);
                default:
                    throw new ApplicationException();
            }
        }


        public static bool IsLeftArgument(int i,
                    Solving[] Tokens)
        {
            if (i < 0)
                return false;

            switch (Tokens[i])
            {
                case Solving.Value:
                case Solving.ClosedParenthesis:
                case Solving.Postfix:
                    return true;
                case Solving.Binary:
                case Solving.Prefix:
                case Solving.PrefixOrBinary:
                case Solving.OpenParenthesis:
                case Solving.Comma:
                    return false;

                case Solving.PostfixOrBinary:
                    return IsLeftArgument(i - 1, Tokens);
                default:
                    throw new ApplicationException();
            }
        }

        public enum Solving
        {
            Value,
            Comma,
            OpenParenthesis,
            ClosedParenthesis,
            Prefix,
            Postfix,
            Binary,
            PrefixOrBinary,
            PostfixOrBinary
        }

        public static IEnumerable<DiscriminatedOperatorToken<TToken, TOperator>> Solve2<TToken, TOperator, TOperatorKey>(
            TToken[] Tokens,
            Func<TToken, TOperatorKey> TokenOperatorSelector,
            IEnumerable<TOperator> Operators
            )
            where TOperator : IArgPosOperator, IIsParenthesis, IIsComma, IOriginalToken<TOperatorKey>
            where TToken : ISubstring 

        {
            //Initialize the operator dictionary:
            var OperatorDic = new Dictionary<TOperatorKey, List<TOperator>>();
            foreach (var Op in Operators)
            {
                var key = Op.OriginalToken;
                List<TOperator> OpList;
                if (!OperatorDic.TryGetValue(key, out OpList))
                {
                    OpList = new List<TOperator>();
                    OperatorDic.Add(key, OpList);
                }
                OpList.Add(Op);

                if (OpList.Count == 3)
                    throw new ArgumentException("Can't handle triple operator discrimination on '" + key.ToString() + "'");
                if (OpList.Count == 2 && !OpList.Any((x) => x.ArgumentPosition == OperatorArgumentPosition.Binary))
                    throw new ArgumentException("Can't handle prefix/postfix operator discrimination on '" + key.ToString() + "'");
            }

            //****************************************************************************
            //Presolve all operators onto an array of matches;
            Solving[] Solving = new Solving[Tokens.Length];
            var MatchArray = new List<TOperator>[Tokens.Length];
            for (int i = 0; i < Tokens.Length; i++)
            {
                List<TOperator> Match;
                if (OperatorDic.TryGetValue(TokenOperatorSelector(Tokens[i]), out Match))
                {
                    MatchArray[i] = Match;
                    if (Match.Count == 1)
                    {
                        if (Match[0].IsOpenParenthesis)
                            Solving[i] = OperatorSolver.Solving.OpenParenthesis;
                        else if (Match[0].IsClosedParenthesis)
                            Solving[i] = OperatorSolver.Solving.ClosedParenthesis;
                        else if (Match[0].IsComma)
                            Solving[i] = OperatorSolver.Solving.Comma;
                        else
                        {
                            switch (Match[0].ArgumentPosition)
                            {
                                case OperatorArgumentPosition.Binary: Solving[i] = OperatorSolver.Solving.Binary; break;
                                case OperatorArgumentPosition.PrefixUnary: Solving[i] = OperatorSolver.Solving.Prefix; break;
                                case OperatorArgumentPosition.PostfixUnary: Solving[i] = OperatorSolver.Solving.Postfix; break;
                            }
                        }
                    }
                    else
                    {
                        if (Match.Any((x) => x.ArgumentPosition == OperatorArgumentPosition.PrefixUnary))
                            Solving[i] = OperatorSolver.Solving.PrefixOrBinary;
                        else
                            Solving[i] = OperatorSolver.Solving.PostfixOrBinary;
                    }
                }
                else
                    Solving[i] = OperatorSolver.Solving.Value;
            }

            //****************************************************************************
            for (int i = 0; i < Tokens.Length; i++)
            {
                if (Solving[i] == OperatorSolver.Solving.Value)
                    yield return CreateDOT<TToken, TOperator>(Tokens[i]);
                else if (MatchArray[i].Count == 1)
                    yield return CreateDOT<TToken, TOperator>(Tokens[i], MatchArray[i][0]);
                else
                {
                    bool LeftArg = IsLeftArgument(i - 1, Solving);
                    bool RightArg = IsRightArgument(i + 1, Solving);

                    var BinaryOp = MatchArray[i].First((x) => x.ArgumentPosition == OperatorArgumentPosition.Binary);
                    var OtherOp = MatchArray[i].First((x) => x.ArgumentPosition != OperatorArgumentPosition.Binary);

                    if (LeftArg && RightArg)
                    {
                        Solving[i] = OperatorSolver.Solving.Binary;
                        yield return CreateDOT(Tokens[i], BinaryOp);
                    }
                    else
                    {
                        if (OtherOp.ArgumentPosition == OperatorArgumentPosition.PostfixUnary)
                            Solving[i] = OperatorSolver.Solving.Postfix;
                        else
                            Solving[i] = OperatorSolver.Solving.Prefix;
                        yield return CreateDOT(Tokens[i], OtherOp);
                    }
                }

            }
        }


    }
}
