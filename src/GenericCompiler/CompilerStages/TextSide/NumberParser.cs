using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumerableExtensions;
namespace GenericCompiler.CompilerStages.TextSide
{
    public static class NumberParser
    {

        /// <summary>
        /// Converts a collection of splitted words onto a collection ITokenSubstring of bool where numbers are single items
        /// with the token value = true
        /// </summary>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static IEnumerable<ITokenSubstring<bool>> ParseNumbers(IEnumerable<ISubstring> Words)
        {
            Func<string, bool> IsInteger = (x) => x.All((c) => char.IsDigit(c));
            Func<string, bool> IsIntegerDot = (x) => x.Length > 1 && IsInteger(x.Substring(0, x.Length - 1)) && x[x.Length - 1] == '.';
            Func<string, bool> IsDecimal = (x) =>
                {

                    int dotIndex = x.IndexOf('.');
                    if (dotIndex == -1 || dotIndex == x.Length - 1)
                        return false;
                    else return IsInteger(x.Substring(0, dotIndex)) && IsInteger(x.Substring(dotIndex + 1, x.Length - dotIndex - 1));
                };
            Func<string, bool> IsNumeric = (x) => IsInteger(x) || IsDecimal(x);

            return Words.AggregateAdjacents(
                (a, b) =>
                        (IsInteger(a.Substring()) && b.Substring() == ".") ||
                        (IsIntegerDot(a.Substring()) && IsInteger(b.Substring()))
                , (a, b) => a.Concat(b)).Select((x) => x.AsToken(IsNumeric(x.Substring())));
        }
    }
}
