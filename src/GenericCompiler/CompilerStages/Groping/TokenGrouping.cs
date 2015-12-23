using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.AbstractTree;
namespace GenericCompiler.CompilerStages.MultiGroping
{
    public static class TokenGrouping
    {
        /// <summary>
        /// Convert a neasted parenthesis group on a form 'func(a b bar(c foo(d e) ) )' on a call tree on the same form, in wich the tree parent values are caller names
        /// </summary>
        /// <typeparam name="TToken"></typeparam>
        /// <typeparam name="TEnclose"></typeparam>
        /// <param name="Head"></param>
        /// <param name="Group"></param>
        /// <returns></returns>
        public static ITree<TOut>[] ToCallTree<TToken, TEnclose, TOut>(this IEnumerable<GroupToken<TToken, TEnclose>> Group, Func<TToken, TOut> TokenSelector)
        {
            Stack<ITree<TOut>> Stack = new Stack<ITree<TOut>>();


            foreach (var G in Group)
            {
                if (G.IsLeaf)
                    Stack.Push(TreeExtensions.CreateLeaf(TokenSelector(G.LeafValue)));
                else
                {
                    if (Stack.Count == 0)
                        throw new ArgumentException("Wrong call tree hierarchy: Can't have a group on the first element");
                    if (!Stack.Peek().IsLeaf())
                        throw new ArgumentException("Wrong call tree hiearchy: The element before of the group must be a leaf ");
                    var Q = Stack.Pop();
                    Stack.Push(TreeExtensions.CreateTree(Q.Value, ToCallTree(G.Subitems, TokenSelector)));
                }
            }
            return Stack.Reverse().ToArray();
        }

       
        public static List<GroupToken<TInput, TEnclose>> GroupTokens<TInput, TEnclose>(
            this TokenGroupDirective<TEnclose> GrouppingDirective,
            IEnumerable<TInput> Tokens,
            Func<TInput, TEnclose> EncloseSelector
            )
            where TInput : ISubstring
            where TEnclose : IEquatable<TEnclose>
        {
            var CurrentGroup = new Stack<GroupToken<TInput, TEnclose>>();
            var CurrentDirective = new Stack<TokenGroupDirective<TEnclose>>();

            CurrentGroup.Push(new GroupToken<TInput, TEnclose>(default(TEnclose), true, new List<GroupToken<TInput, TEnclose>>()));
            CurrentDirective.Push(GrouppingDirective);

            foreach (var item in Tokens)
            {
                var CD = CurrentDirective.Peek();
                var Selector = CurrentDirective.Peek().GroupSelector;
                var itemEnclose = EncloseSelector(item);
                bool excluded;
                GroupTokenType Type;
                if (itemEnclose == null || !Selector.TryGetValue(itemEnclose, out Type))
                {
                    CurrentGroup.Peek().Subitems.Add(new GroupToken<TInput, TEnclose>(item));
                }
                else
                {
                    switch (Type)
                    {
                        case GroupTokenType.Begin:
                            {
                                excluded = false;
                                if (CD.ExclusiveEnclosers.Contains(itemEnclose))
                                {
                                    CurrentGroup.Peek().Subitems.Add(new GroupToken<TInput, TEnclose>(item));
                                    excluded = true;
                                }
                                var NewGroup = new GroupToken<TInput, TEnclose>(itemEnclose, excluded, new List<GroupToken<TInput, TEnclose>>());


                                CurrentGroup.Peek().Subitems.Add(NewGroup);
                                CurrentGroup.Push(NewGroup);
                                CurrentDirective.Push(CD.SubDirective[itemEnclose]);

                                break;
                            }
                        case GroupTokenType.End:
                            {
                                var CG = CurrentGroup.Peek();
                                CG.EndEncloser = itemEnclose;
                                CG.EndExcluded = CD.ExclusiveEnclosers.Contains(itemEnclose);

                                if (CurrentGroup.Count == 1)
                                    throw new CompilerException("Groupping end without begin on '" + CG.EndEncloser.ToString() + "'", item);

                                if (!CurrentDirective.Peek().BeginEndMatch[CG.BeginEncloser].Equals(CG.EndEncloser))
                                    throw new CompilerException("Groupping end '" + CG.EndEncloser.ToString() + "' mistmatch with '" + CG.BeginEncloser.ToString() + "'", item);
                                CurrentGroup.Pop();
                                CurrentDirective.Pop();

                                if (CG.EndExcluded)
                                    CurrentGroup.Peek().Subitems.Add(new GroupToken<TInput, TEnclose>(item));

                                break;
                            }
                    }
                }
            }

            if (CurrentGroup.Count != 1) throw new CompilerException("Groupping mismatch", Tokens.Aggregate((a, b) => (TInput)a.Concat(b)));
            return CurrentGroup.Peek().Subitems;
        }


        /// <summary>

        private static TokenGroupDirective<TInput> CreateChildlessGroup<TInput>
            (
            IEnumerable<Tuple<TInput, TInput>> GroupEnclosing,
            bool AddBegins, bool AddEnds
            )
            where TInput : IEquatable<TInput>
        {
            TokenGroupDirective<TInput> Ret = new TokenGroupDirective<TInput>();


            Ret.GroupSelector = new Dictionary<TInput, GroupTokenType>();
            Ret.BeginEndMatch = new Dictionary<TInput, TInput>();
            foreach (var x in GroupEnclosing)
            {
                if (AddBegins)
                    Ret.GroupSelector.Add(x.Item1, GroupTokenType.Begin);
                if (AddEnds)
                    Ret.GroupSelector.Add(x.Item2, GroupTokenType.End);
                Ret.BeginEndMatch.Add(x.Item1, x.Item2);
            }

            return Ret;
        }

        /// <summary>
        /// Create a multigroup where every group type can be inside all others
        /// </summary>
        public static TokenGroupDirective<TInput> AsNeastedGroup<TInput>(this IEnumerable<Tuple<TInput, TInput>> GroupEnclosing) where TInput : IEquatable<TInput>
        {
            var Ret = CreateChildlessGroup(GroupEnclosing, true, true);
            Ret.SubDirective = new Dictionary<TInput, TokenGroupDirective<TInput>>();
            Ret.ExclusiveEnclosers = new HashSet<TInput>();
            foreach (var x in GroupEnclosing)
                Ret.SubDirective.Add(x.Item1, Ret);
            return Ret;
        }

        /// <summary>
        /// Each group ignores all other subgroups, including self and only ending with its specified group end
        /// </summary>
        /// <returns></returns>
        public static TokenGroupDirective<TInput> CreateFlatGroup<TInput>
            (
                IEnumerable<Tuple<TInput, TInput>> GroupEnclosing,
                IEnumerable<TInput> ExclusiveEnclosers
            )
            where TInput : IEquatable<TInput>
        {
            var Ret = CreateChildlessGroup(GroupEnclosing, true, false);

            var excEnc = new HashSet<TInput>(ExclusiveEnclosers);

            Ret.SubDirective = new Dictionary<TInput, TokenGroupDirective<TInput>>();
            Ret.ExclusiveEnclosers = excEnc;
            foreach (var x in GroupEnclosing)
            {
                var SRet = new TokenGroupDirective<TInput>();
                SRet.GroupSelector = new Dictionary<TInput, GroupTokenType>();
                SRet.GroupSelector.Add(x.Item2, GroupTokenType.End);

                SRet.BeginEndMatch = new Dictionary<TInput, TInput>();
                SRet.BeginEndMatch.Add(x.Item1, x.Item2);

                SRet.ExclusiveEnclosers = excEnc;

                SRet.SubDirective = null;

                Ret.SubDirective.Add(x.Item1, SRet);
            }
            return Ret;
        }
    }

    public enum GroupTokenType
    {
        Begin,
        End,
    }

    public class TokenGroupDirective<TEnclose>
    {
        /// <summary>
        /// A dictionary of begin-end tokens
        /// </summary>
        public Dictionary<TEnclose, GroupTokenType> GroupSelector { get; set; }
        /// <summary>
        /// A dictionary of begin-end matches
        /// </summary>
        public Dictionary<TEnclose, TEnclose> BeginEndMatch { get; set; }

        /// <summary>
        /// Group enclosers that are included on subitems instead of group begin/ends
        /// </summary>
        public HashSet<TEnclose> ExclusiveEnclosers { get; set; }

        /// <summary>
        /// Child subdirective for a given group begin
        /// </summary>
        public Dictionary<TEnclose, TokenGroupDirective<TEnclose>> SubDirective { get; set; }



    }

    [DebuggerDisplay("{ToString()}")]
    public class GroupToken<TToken, TEnclose>
    {
        public GroupToken(TToken LeafValue)
        {
            this.LeafValue = LeafValue;
            this.Subitems = null;
        }

        public GroupToken(TEnclose BeginToken, bool BeginExcluded, List<GroupToken<TToken, TEnclose>> Subitems)
        {
            this.BeginEncloser = BeginToken;
            this.BeginExcluded = BeginExcluded;
            this.Subitems = Subitems;
        }


        /// <summary>
        /// Returns true if subitems == null
        /// </summary>
        public bool IsLeaf
        {
            get
            { return Subitems == null; }
        }

        /// <summary>
        /// Valid only for leaf values
        /// </summary>
        public TToken LeafValue { get; private set; }

        /// <summary>
        /// Begin token of a group. Only valid for non-leaf tokens
        /// </summary>
        public TEnclose BeginEncloser { get; private set; }

        /// <summary>
        /// Return true if the begin token was excluded from the group
        /// </summary>
        public bool BeginExcluded { get; private set; }

        /// <summary>
        /// End token of a group. Only valid for non-leaf tokens
        /// </summary>
        public TEnclose EndEncloser { get; set; }
        /// <summary>
        /// Return true if the end token was excluded from the group
        /// </summary>
        public bool EndExcluded { get; set; }


        /// <summary>
        /// Subitems of a non-leaf token
        /// </summary>
        public List<GroupToken<TToken, TEnclose>> Subitems { get; private set; }

        public static implicit operator TToken(GroupToken<TToken, TEnclose> Token)
        {
            if (Token.IsLeaf)
                return Token.LeafValue;
            else
                throw new ArgumentException("Only leaf values can be converted");
        }

        public override string ToString()
        {
            if (IsLeaf)
                return LeafValue.ToString();
            else if (Subitems.Count > 0)
                return
                    (BeginExcluded ? "" : BeginEncloser.ToString())
                    + Subitems.Select((x) => x.ToString()).Aggregate((a, b) => a + b)
                    + (EndExcluded ? "" : EndEncloser.ToString());
            else
                return
                            (BeginExcluded ? "" : BeginEncloser.ToString())
                            + (EndExcluded ? "" : EndEncloser.ToString());
        }


    }
}
