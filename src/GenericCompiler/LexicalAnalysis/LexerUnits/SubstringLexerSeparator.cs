using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.LexicalAnalysis.LexerUnits.Parsers;
namespace GenericCompiler.LexicalAnalysis.LexerUnits
{
    public class SubstringLexerSeparator
    {
        public SubstringLexerSeparator(IEnumerable<ITokenizedParser> Units)
        {
            this.Units = new List<ITokenizedParser>(Units);
        }
        public List<ITokenizedParser> Units;

        public LexerWordLenght FindValidLeaf(string Text, int index, int count)
        {
            int len = 0;
            int maxValidLen = 0;
            List<ITokenizedParser> MaxValidUnits = new List<ITokenizedParser>();

            bool anyPosible = true;
            while (anyPosible && len < count)
            {
                len++;
                anyPosible = false;
                foreach (var U in Units)
                {
                    if (U.IsPosible() || U.IsValid())
                    {
                        U.Append(Text[index]);

                        if (U.IsPosible())
                            anyPosible = true;
                    }
                    if (U.IsValid())
                    {
                        if (len > maxValidLen)
                        {
                            MaxValidUnits.Clear();
                            maxValidLen = len;
                        }
                        MaxValidUnits.Add(U);
                    }
                }

                index++;
            }
            if (MaxValidUnits.Count > 1)
                throw new ApplicationException("Can't diferentiate between valid units");
            else if (MaxValidUnits.Count == 1)
                return new LexerWordLenght(maxValidLen, MaxValidUnits[0].Token);
            else
                return new LexerWordLenght(1, Guid.Empty);
        }

        public List<LexerWordLenght> Split(string Text)
        {
            return Split(new Substring(Text));
        }
        public List<LexerWordLenght> Split(ISubstring Text)
        {
            var Ret = new List<LexerWordLenght>();

            int index = Text.CharIndex;
            int lastIndex = Text.CharIndex + Text.CharLen;

            int UnidentifiedWordLen = 0;
            for (int i = index; i < lastIndex; )
            {
                //Reset all units
                foreach (var U in Units) U.Reset();

                //Find the next largest leaf:
                var AdvanceWord = FindValidLeaf(Text.CompleteString, i, lastIndex - i);

                if (AdvanceWord.Token == Guid.Empty)
                    UnidentifiedWordLen++;
                else
                {
                    if (UnidentifiedWordLen > 0)
                    {
                        Ret.Add(new LexerWordLenght(UnidentifiedWordLen, Guid.Empty));
                        UnidentifiedWordLen = 0;
                    }

                    Ret.Add(AdvanceWord);
                }

                i += AdvanceWord.Lenght;
            }

            if (UnidentifiedWordLen > 0)
                Ret.Add(new LexerWordLenght(UnidentifiedWordLen, Guid.Empty));

            return Ret;
        }


    }
}
