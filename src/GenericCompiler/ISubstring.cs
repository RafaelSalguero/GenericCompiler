using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler
{
    /// <summary>
    /// A part of a string
    /// </summary>
    public interface ISubstring : IEquatable<ISubstring>
    {
        string CompleteString { get; }
        /// <summary>
        /// Start index of the substring
        /// </summary>
        int CharIndex { get; }
        /// <summary>
        /// Lenght of the substring
        /// </summary>
        int CharLen { get; }
    }

    /// <summary>
    /// A compiler item that has been generated from a given token on early compiler stages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOriginalToken <T>
    {
        T OriginalToken { get; }
    }

    /// <summary>
    /// A class that has been cathegorized with a given token type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IToken<T>
    {
        T Token { get; }
    }

    /// <summary>
    /// A substring that contains a categorized token identifier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITokenSubstring<T> : ISubstring, IToken<T>
    {
    }

    [DebuggerDisplay("{ToString()}")]
    public class TokenSubstring<T> : Substring, ITokenSubstring<T>
    {
        public TokenSubstring(ISubstring other, T Token)
            : base(other)
        {
            this.token = Token;
        }

        private readonly T token;
        public T Token
        {
            get { return token; }
        }

        public override string ToString()
        {
            return "[" + Token.ToString() + "] " + base.ToString();
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class Substring : ISubstring
    {
        public Substring(ISubstring Other) : this(Other.CompleteString, Other.CharIndex, Other.CharLen) { }
        public Substring(string CompleteString, int CharIndex, int CharLen)
        {
            this.completeString = CompleteString;
            this.charIndex = CharIndex;
            this.charLen = CharLen;
        }

        private readonly string completeString;
        public string CompleteString
        {
            get { return completeString; }
        }

        private readonly int charIndex;
        public int CharIndex
        {
            get { return charIndex; }
        }
        private readonly int charLen;
        public int CharLen
        {
            get { return charLen; }
        }

        public bool Equals(ISubstring other)
        {
            return this.EqualsSubstring(other);
        }

        public override string ToString()
        {
            return this.Substring();
        }
    }

    public static class ISubstringExtensions
    {
        public static ISubstring Concat(this ISubstring a, ISubstring b)
        {
            if (!Object.ReferenceEquals(a.CompleteString, b.CompleteString))
                throw new ArgumentException("ParserPositions are from diferent codelines");
            if (b.CharIndex != a.CharIndex + a.CharLen)
                throw new ArgumentException("ParserPositions arent contigous");
            return new Substring(a.CompleteString, a.CharIndex, a.CharLen + b.CharLen);
        }

        /// <summary>
        /// Create a ITokenSubstring from a given substring
        /// </summary>
        /// <returns></returns>
        public static ITokenSubstring<T> AsToken<T>(this ISubstring S, T Token)
        {
            return new TokenSubstring<T>(S, Token);
        }

        public static ISubstring BeforeSelf(this ISubstring S)
        {
            return new Substring(S.CompleteString, 0, S.CharIndex + 1 - S.CharLen);
        }
        public static ISubstring AfterSelf(this ISubstring S)
        {
            return new Substring(S.CompleteString, S.CharIndex + S.CharLen, S.CompleteString.Length - S.CharIndex - S.CharLen);
        }

        public static ISubstring AsSubstring(this string s, int charIndex, int charLen)
        {
            return new Substring(s, charIndex, charLen);
        }
        public static ISubstring AsSubstring(this string s)
        {
            return new Substring(s, 0, s.Length);
        }

        public static bool EqualsSubstring(this ISubstring S, ISubstring other)
        {
            if (object.ReferenceEquals(S.CompleteString, other.CompleteString) &&
                S.CharIndex == other.CharIndex &&
                S.CharLen == other.CharLen)
                return true;
            else
                return S.Substring() == other.Substring();
        }
        public static string Substring(this ISubstring S)
        {
            return S.CompleteString.Substring(S.CharIndex, S.CharLen);
        }
    }
}
