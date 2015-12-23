using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler
{
    /// <summary>
    /// A dictionary of token friendly names
    /// </summary>
    internal static class GuidNames
    {
        private static Dictionary<Guid, string> Names = InitNames();
        private static Dictionary<Guid, string> InitNames()
        {
            var Ret = new Dictionary<Guid, string>();
            Ret.Add(Guid.Empty, "[any]");
            return Ret;
        }
        internal static void AddToken(Guid Token, string Name)
        {
            if (Names.ContainsKey(Token))
                Names.Remove(Token);
            Names.Add(Token, Name);
        }
        internal static string GetName(Guid Token)
        {
            if (!Names.ContainsKey(Token))
                return Token.ToString();
            return Names[Token];
        }
        internal static bool HasName(Guid Token)
        {
            return Names.ContainsKey(Token);
        }
    }
}
