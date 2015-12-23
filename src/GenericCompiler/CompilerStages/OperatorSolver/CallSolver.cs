using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.CompilerStages.OperatorSolver
{
    public static class CallSolver
    {
        public enum ArgSeparatorType
        {
            Word,
            OpenParenthesis,
            ClosingParenthesis,
            Comma
        }
        public static List<List<T>> ExtractArguments<T>(T[] Tokens, int firstParenthesisIndex, Func<T, ArgSeparatorType> TypeSelector)
            where T : ISubstring
        {
            int dummy;
            return ExtractArguments(Tokens, firstParenthesisIndex, TypeSelector, out dummy);
        }
        public static List<List<T>> ExtractArguments<T>(T[] Tokens, int firstParenthesisIndex, Func<T, ArgSeparatorType> TypeSelector, out int lastParenthesisIndex)
            where T : ISubstring
        {
            var result = new List<List<T>>();
            var current = new List<T>();

            firstParenthesisIndex++;
            int ParenthesisLevel = 0;
            lastParenthesisIndex = -1;
            for (int i = firstParenthesisIndex; i < Tokens.Length; i++)
            {
                var To = Tokens[i];
                var Type = TypeSelector(To);
                if (Type == ArgSeparatorType.OpenParenthesis)
                {
                    ParenthesisLevel++;
                    current.Add(To);
                }
                else if (Type == ArgSeparatorType.ClosingParenthesis)
                {
                    ParenthesisLevel--;
                    if (ParenthesisLevel == -1)
                    {
                        lastParenthesisIndex = i;
                        if (current.Count > 0)
                            result.Add(current);
                        break;
                    }
                }
                else if (Type == ArgSeparatorType.Comma && ParenthesisLevel == 0)
                {
                    result.Add(current);
                    current = new List<T>();
                }
                else
                    current.Add(Tokens[i]);
            }
            if (lastParenthesisIndex == -1)
            {
                throw new CompilerException("Parenthesis mismatch on argument extraction", Tokens[Tokens.Length - 1]);
            }
            return result;
        }

        public static List<List<T>> ExtractArguments<T>(IEnumerable<T> Tokens, int firstParenthesisIndex, out int lastParenthesisIndex)
            where T : IIsParenthesis, IIsComma, ISubstring
        {
            return ExtractArguments(Tokens.ToArray(), firstParenthesisIndex, (x) =>
            {
                if (x.IsClosedParenthesis)
                    return ArgSeparatorType.ClosingParenthesis;
                else if (x.IsOpenParenthesis)
                    return ArgSeparatorType.OpenParenthesis;
                else if (x.IsComma)
                    return ArgSeparatorType.Comma;
                else
                    return ArgSeparatorType.Word;
            }, out lastParenthesisIndex);
        }
        public static List<List<T>> ExtractArguments<T>(IEnumerable<T> Tokens, int firstParenthesisIndex)
            where T : IIsParenthesis, IIsComma, ISubstring
        {
            int dummy;
            return ExtractArguments(Tokens, firstParenthesisIndex, out dummy);
        }

        public static IEnumerable<T> InsertCallSolver<T>(T[] Tokens, Func<int, T> CreateArgNumCall)
            where T : IIsParenthesis, IIsComma, IIsOperator, ISubstring
        {
            for (int i = 0; i < Tokens.Length; i++)
            {
                yield return Tokens[i];
                if (i < Tokens.Length - 1 && !Tokens[i].IsOperator && Tokens[i + 1].IsOpenParenthesis)
                {
                    var arguments = ExtractArguments(Tokens, i + 1).ToArray();
                    yield return CreateArgNumCall(arguments.Length + 1);
                }
            }
        }
    }
}
