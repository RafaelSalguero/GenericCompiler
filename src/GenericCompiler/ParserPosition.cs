using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler
{
    //[DebuggerDisplay("{ToString()}")]
    //public struct ParserPosition : ILocalizableToken, IEquatable<ParserPosition>
    //{
    //    public ParserPosition(string codeLine)
    //        : this(codeLine, 0, codeLine.Length)
    //    { }
    //    public ParserPosition(string codeLine, int charIndex, int charLen)
    //    {
    //        this.codeLine = codeLine;
    //        this.charIndex = charIndex;
    //        this.charLen = charLen;
    //    }

    //    /// <summary>
    //    /// Original codeline reference
    //    /// </summary>
    //    public readonly string codeLine;
    //    public string Substring
    //    {
    //        get
    //        {
    //            return codeLine.Substring(charIndex, charLen);
    //        }
    //    }

    //    public ParserPosition BeforeSelf
    //    {
    //        get
    //        {
    //            return new ParserPosition(codeLine, 0, charIndex + 1 - charLen);
    //        }
    //    }
    //    public ParserPosition AfterSelf
    //    {
    //        get
    //        {
    //            return new ParserPosition(codeLine, charIndex + charLen, codeLine.Length - charIndex - charLen);
    //        }
    //    }

    //    public readonly int charIndex, charLen;

    //    public static implicit operator ParserPosition(string text)
    //    {
    //        return new ParserPosition(text);
    //    }

    //    public static ParserPosition operator +(ParserPosition a, ParserPosition b)
    //    {
    //        if (!Object.ReferenceEquals(a.codeLine, b.codeLine))
    //            throw new ArgumentException("ParserPositions are from diferent codelines");
    //        if (b.charIndex != a.charIndex + a.charLen)
    //            throw new ArgumentException("ParserPositions arent contigous");
    //        return new ParserPosition(a.codeLine, a.charIndex, a.charLen + b.charLen);
    //    }

    //    public ParserPosition CompleteLine
    //    {
    //        get
    //        {
    //            return new ParserPosition(codeLine, 0, codeLine.Length);
    //        }
    //    }
    //    public override string ToString()
    //    {
    //        return Substring;
    //    }

    //    public ParserPosition Position
    //    {
    //        get { return this; }
    //    }

    //    public static bool operator ==(ParserPosition a, ParserPosition b)
    //    {
    //        return a.Equals(b);
    //    }
    //    public static bool operator !=(ParserPosition a, ParserPosition b)
    //    {
    //        return !(a == b);
    //    }

    //    public bool Equals(ParserPosition other)
    //    {
    //        if (object.ReferenceEquals(this.codeLine, other.codeLine) &&
    //            this.charIndex == other.charIndex &&
    //            this.charLen == other.charLen)
    //            return true;
    //        else
    //            return Substring == other.Substring;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is ParserPosition)
    //            return Equals((ParserPosition)obj);
    //        else
    //            return false;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return this.charIndex ^ (this.charLen * 1234545);
    //    }
    //}

}
