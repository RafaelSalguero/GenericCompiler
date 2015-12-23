using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.TextSide
{
    public static class WordSplitter
    {
        public static IEnumerable<ISubstring> SplitWords(this string Text, IEnumerable<string> Separators)
        {
            return SplitWords(Text.AsSubstring(), Separators);
        }
        public static IEnumerable<ISubstring> SplitWords(this ISubstring Text, IEnumerable<string> Separators)
        {
            var Matcher = WordMatcherTree.CreateTree(Separators);

            WordMatcherTree CurrentTree = Matcher, LastTree = null;
            int acum = 0;
            for (int i = Text.CharIndex; i <= Text.CharIndex + Text.CharLen; i++)
            {
                var C = i < (Text.CharIndex + Text.CharLen) ? Text.CompleteString[i] : (char)0;

                CurrentTree = CurrentTree == null ? Matcher.GetChar(C) : CurrentTree.GetChar(C);
                if (CurrentTree == null)
                {
                    if (LastTree != null)
                    {
                        int FullLen, MatchLen;
                        LastTree.GetLens(out FullLen, out MatchLen);
                        int MismatchLen = FullLen - MatchLen;
                        if (MatchLen == 0)
                        {
                            acum += MismatchLen;
                            i -= 1;
                        }
                        else
                        {
                            //Add the acum:
                            if (acum > 0)
                            {
                                //Not a separator:
                                yield return Text.CompleteString.AsSubstring(i - acum - FullLen, acum);
                                acum = 0;
                            }

                            //Add the current matched word:
                            if (MatchLen > 0)
                            {
                                //Separator:
                                yield return Text.CompleteString.AsSubstring(i - FullLen, MatchLen);
                            }

                            i -= MismatchLen + 1;
                        }
                    }
                    else
                    {
                        acum++;
                    }
                }
                else
                {
                    if (CurrentTree.Terminal)
                    {
                        int len = CurrentTree.WordTracebackLenght;
                        len = len;
                    }
                }

                LastTree = CurrentTree;
            }

            Debug.Assert(acum >= 1, "acum fue menor que 1");
            if (acum > 1)
            {
                //Not a separator:
                yield return Text.CompleteString.AsSubstring(Text.CharIndex + Text.CharLen - acum + 1, acum - 1);
            }
        }

        /// <summary>
        /// A word matcher tree is used for matching words that can contain the same prefix on an advancing string 
        /// </summary>
        private class WordMatcherTree
        {
            public static WordMatcherTree CreateTree(IEnumerable<string> Words)
            {
                WordMatcherTree Tree = new WordMatcherTree();
                WordMatcherTree CurrentTree;
                foreach (var W in Words)
                {
                    CurrentTree = Tree;
                    foreach (var C in W)
                        CurrentTree = CurrentTree.AddChar(C);
                    CurrentTree.Match = true;
                }
                return Tree;
            }

            public Dictionary<char, WordMatcherTree> Subitems = new Dictionary<char, WordMatcherTree>();
            public WordMatcherTree Parent;
            public char Key;
            public bool Match;
            public bool Terminal
            {
                get
                {
                    return Subitems.Count == 0;
                }
            }
            public bool IsRoot
            {
                get
                {
                    return Parent == null;
                }
            }
            public string WordTraceback
            {
                get
                {
                    WordMatcherTree current = this;
                    StringBuilder B = new StringBuilder();
                    while (current.Parent != null)
                    {
                        B.Insert(0, current.Key);
                        current = current.Parent;
                    }
                    return B.ToString();
                }
            }
            public int WordTracebackLenght
            {
                get
                {
                    int a, b;
                    GetLens(out a, out b);
                    return a;
                }
            }
            public int MatchTracebackLenght
            {
                get
                {
                    int a, b;
                    GetLens(out a, out b);
                    return b;
                }
            }

            public void GetLens(out int FullLenght, out int MatchLenght)
            {
                WordMatcherTree current = this;
                bool Matched = current.Match;
                FullLenght = 0;
                MatchLenght = 0;
                while (current.Parent != null)
                {
                    current = current.Parent;
                    if (Matched)
                        MatchLenght++;
                    else
                        Matched = current.Match;
                    FullLenght++;
                }
            }



            public WordMatcherTree this[char C]
            {
                get
                {
                    return GetChar(C);
                }
            }
            public WordMatcherTree this[string word]
            {
                get
                {
                    return GetWord(word);
                }
            }

            public WordMatcherTree GetWord(string Word)
            {
                WordMatcherTree Current = this;
                foreach (var C in Word)
                {
                    Current = Current.GetChar(C);
                    if (Current == null)
                        return null;
                }
                return Current;
            }

            public WordMatcherTree GetChar(char C)
            {
                WordMatcherTree ret;
                if (Subitems.TryGetValue(C, out ret))
                    return ret;
                else
                    return null;
            }

            public WordMatcherTree AddChar(char C)
            {
                WordMatcherTree ret;
                if (Subitems.TryGetValue(C, out ret))
                    return ret;
                else
                {
                    ret = new WordMatcherTree();
                    ret.Parent = this;
                    ret.Key = C;
                    Subitems.Add(C, ret);
                    return ret;
                }
            }

            public override string ToString()
            {
                var B = new StringBuilder();
                if (Match)
                    B.Append("*");
                foreach (var C in Subitems)
                {
                    B.Append(C.Key + " => {" + C.Value.ToString() + "} ");
                }
                return B.ToString();
            }
        }
    }
}
