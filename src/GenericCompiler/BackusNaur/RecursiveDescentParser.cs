using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.BackusNaur.Backus;
using GenericCompiler.BackusNaur.ParserResult;
using EnumerableExtensions;
namespace GenericCompiler.BackusNaur
{
    /// <summary>
    /// An exception of the parsing phase of the compiler
    /// </summary>
    public class ParserException : CompilerException
    {
        public ParserException(string Message, ISubstring Position)
            : base(Message, Position) { }

        public ParserException(string Message, ISubstring Position, Exception InnerException)
            : base(Message, Position, InnerException) { }
    }

    internal class UnexpectedSymbolException : ParserException
    {
        public UnexpectedSymbolException(string Expected, ISubstring Position)
            : base("The symbol '" + Expected.ToString() + "' was expected instead of '" + Position.Substring() + "'", Position)
        {
        }

        static internal Expression New(Guid Expected, Expression Symbols)
        {
            var Position = Expression.Convert(Expression.Call(Symbols, "Peek", null), typeof(ISubstring));
            string TokenName = GuidNames.GetName(Expected);
            return Expression.New(
                typeof(UnexpectedSymbolException).GetConstructor(new Type[] { typeof(string), typeof(ISubstring) }),
                Expression.Constant(TokenName),
                Position);
        }
    }

    internal class UnexpectedEndingException : ParserException
    {
        public UnexpectedEndingException(string Expected, ISubstring Position)
            : base("The symbol '" + Expected.ToString() + "' was expected instead of the end of file", Position)
        {
        }

        static internal Expression New(Guid Expected, Expression Symbols)
        {
            var Position = Expression.Convert(Expression.Call(Symbols, "AbsoluteLast", null), typeof(ISubstring));
            string TokenName = GuidNames.GetName(Expected);
            return Expression.New(
                typeof(UnexpectedEndingException).GetConstructor(new Type[] { typeof(string), typeof(ISubstring) }),
                Expression.Constant(TokenName),
                Position);
        }
    }

    internal class EndingExpectedException : ParserException
    {
        public EndingExpectedException(ISubstring Position)
            : base("The end of file was expected instead of '" + Position.Substring() + "'", Position)
        {
        }

        static internal Expression New(Expression Symbols)
        {
            var Position = Expression.Convert(Expression.Call(Symbols, "AbsoluteLast", null), typeof(ISubstring));
            return Expression.New(typeof(EndingExpectedException).GetConstructor(new Type[] { typeof(ISubstring) }), Position);
        }
    }

    internal class SequenceException : ParserException
    {
        public SequenceException(string Name, string Before, string Expected, ISubstring Position, ParserException InnerException)
            : base(
            string.IsNullOrEmpty(Before) ?
            ("At the beginning of the sequence '" + Name + "' " + InnerException.Message) :
            ("In the sequence " + Name + " after the tokens '" + Before + "' " + InnerException.Message), Position)
        {

        }

        static internal Expression New(string Before, string TokenName, string SequenceName, Expression Symbols, Expression Inner)
        {
            var Position =
                Expression.Convert(Expression.Call(Symbols, "PeekOrAbsoluteLast", null), typeof(ISubstring));
            return Expression.New(typeof(SequenceException).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(string), typeof(ISubstring), typeof(ParserException) }),
               Expression.Constant(SequenceName),
               Expression.Constant(Before),
                Expression.Constant(TokenName),
                Position,
                Inner
                );
        }
    }

    internal class EmptySequencesNotAllowedException : ParserException
    {
        public EmptySequencesNotAllowedException(string Name, ISubstring Position, ParserException InnerException)
            : base("The sequence " + Name + " can't be empty " + InnerException.Message, Position)
        {
        }

        static internal Expression New(string ExpectedTokenName, string SequenceName, Expression Symbols, Expression Inner)
        {
            var Position =
                Expression.Convert(Expression.Call(Symbols, "PeekOrAbsoluteLast", null), typeof(ISubstring));
            return Expression.New(typeof(EmptySequencesNotAllowedException).GetConstructor(new Type[] { typeof(string), typeof(ISubstring), typeof(ParserException) }),
              Expression.Constant(SequenceName),
               Position,
               Inner
               );
        }
    }

    internal class CasesException : ParserException
    {
        public CasesException(string Cases, ISubstring Position, IEnumerable<ParserException> InnerExceptions)
            : base("One of the given expressions was expected: " + Cases + ": " + InnerExceptions.Select((y) => y.Message).ToStringEnum(" or "), Position, new AggregateException(InnerExceptions))
        {

        }

        static internal Expression New(string Cases, Expression Symbols, Expression ExList)
        {
            var Position =
                Expression.Convert(Expression.Call(Symbols, "PeekOrAbsoluteLast", null), typeof(ISubstring));
            return Expression.New(typeof(CasesException).GetConstructor(new Type[] { typeof(string), typeof(ISubstring), typeof(IEnumerable<ParserException>) }),
                Expression.Constant(Cases),
                Position,
                ExList
                );
        }
    }

    public class ParserContext
    {
        internal ParserContext(Dictionary<Guid, RecursiveDescentParser.ParserDelegate> Parsers, RecursiveDescentParser.ParserDelegate Method)
        {
            this.Parsers = Parsers;
            this.Method = Method;
        }
        private Dictionary<Guid, RecursiveDescentParser.ParserDelegate> Parsers = new Dictionary<Guid, RecursiveDescentParser.ParserDelegate>();
        private RecursiveDescentParser.ParserDelegate Method;

        /// <summary>
        /// Parse a collection of symbols with this parser
        /// </summary>
        /// <param name="Symbols"></param>
        /// <returns></returns>
        public PExpression Parse(IEnumerable<ITokenSubstring<Guid>> Symbols)
        {
            var Queue = new StateDequeue<ITokenSubstring<Guid>>(Symbols);
            return Method(Queue, this.Parsers);
        }
    }

    public static class RecursiveDescentParser
    {
        public delegate Expression CreateIntermediateParserDelegate(BackusExpression Backus, ParameterExpression Symbols, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail);
        public delegate PExpression ParserDelegate(StateDequeue<ITokenSubstring<Guid>> Symbols, IDictionary<Guid, ParserDelegate> Context);


        internal static Expression Guid(Guid Value)
        {
            return Expression.New(typeof(Guid).GetConstructor(new Type[] { typeof(string) }), Expression.Constant(Value.ToString()));
        }

        public static Expression CreateEndParserEx(End Backus, ParameterExpression Symbols, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            return Expression.IfThen(Expression.Not(Expression.Property(Symbols, "IsEmpty")), Fail(EndingExpectedException.New(Symbols)));
        }

        /// <summary>
        /// Create a terminal parser with a given consume and fail expression
        /// </summary>
        /// <param name="Terminal"></param>
        /// <param name="Symbols"></param>
        /// <param name="Consume"></param>
        /// <param name="Fail"></param>
        /// <returns></returns>
        public static Expression CreateTerminaParserEx(BackusExpression Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            var Terminal = (Terminal)Backus;
            var ExpectedGuid = Guid(Terminal.Token);
            var Peek = Expression.Call(Symbols, "Peek", null);
            var Dequeue = Expression.Call(Symbols, "Dequeue", null);
            var IsEmpty = Expression.Property(Symbols, "IsEmpty");
            TokenSubstring<Guid> a;


            var SubstringMethod = typeof(ISubstringExtensions).GetMethod("Substring", BindingFlags.Public | BindingFlags.Static);

            var CreateResult =
                Terminal.Word ?
                (
                    Expression.New(
                    typeof(WordResult).GetConstructor(new Type[] { typeof(string), typeof(Guid) }),
                    Expression.Call(SubstringMethod, Dequeue),
                     Guid(Terminal.ExpressionId))
                ) :
                (
                    (Expression)Expression.Block(Dequeue,
                    Expression.New(
                    typeof(TerminalResult).GetConstructor(new Type[] { typeof(Guid) }),
                    Guid(Terminal.ExpressionId)))
                );

            var TokenProp = typeof(IToken<Guid>).GetProperties().Where((x) => x.Name == "Token").First();
            var Predicate = Expression.Equal(Expression.Property(Peek, TokenProp), ExpectedGuid);

            var Exception = UnexpectedSymbolException.New(Terminal.Token, Symbols);

            return
                Expression.IfThenElse(IsEmpty, Fail(UnexpectedEndingException.New(Terminal.Token, Symbols)),
                Expression.IfThenElse(Predicate, Consume(CreateResult), Fail(Exception)));
        }

        public static Expression CreateSequenceParserEx(BackusExpression Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            var Sequence = (Sequence)Backus;
            var List = Expression.Parameter(typeof(List<PExpression>));


            var BlockList = new List<Expression>();
            BlockList.Add(Expression.Assign(List, Expression.New(typeof(List<PExpression>))));

            var FailLabel = Expression.Label("SequenceFail");
            var Before = new List<string>();

            foreach (var Ex in Sequence.Expressions)
            {
                BlockList.Add(CreateIntermediateParserEx(Ex, Symbols, Context, ParsersToFill,
                    (x) => Expression.Call(List, "Add", null, x),
                    (x) =>
                        Expression.Block(
                        Fail(SequenceException.New(Before.ToStringEnum(" "), Ex.ToString(), Sequence.ToString(), Symbols, x)),
                        Expression.Goto(FailLabel)
                        )
                        ));

                Before.Add(Ex.ToString());
            }

            var Ret = Expression.New(typeof(CollectionResult).GetConstructor(new Type[] { typeof(IEnumerable<PExpression>), typeof(Guid) }), List, Guid(Backus.ExpressionId));
            BlockList.Add(Consume(Ret));
            BlockList.Add(Expression.Label(FailLabel));


            return Expression.Block(new ParameterExpression[] { List }, BlockList);
        }



        public static Expression CreateCasesParserEx(BackusExpression Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            var Cases = (Cases)Backus;
            var ExList = Expression.Parameter(typeof(List<ParserException>), "ExList");

            var BlockList = new List<Expression>();
            BlockList.Add(Expression.Assign(ExList, Expression.New(typeof(List<ParserException>))));

            int count = 0;
            string CasesText = Cases.ToString();
            foreach (var Case in Cases.Expressions)
            {
                count++;
                BlockList.Add(Expression.Call(Symbols, "PushState", null));
                BlockList.Add(
                    CreateIntermediateParserEx(Case, Symbols, Context, ParsersToFill,
                        (x) =>
                            Expression.Block(
                                Expression.Call(Symbols, "DropState", null),
                                Consume(Expression.New(typeof(SingleResult).GetConstructor(new Type[] { typeof(PExpression), typeof(Guid) }), x, Guid(Backus.ExpressionId)))
                            ),
                        (x) =>
                            Expression.Block(
                            Expression.Call(ExList, "Add", null, x),
                            Expression.Call(Symbols, "PopState", null)
                            )
                        ));
            }

            BlockList.Add(
                Expression.IfThen(Expression.Equal(Expression.Property(ExList, "Count"), Expression.Constant(count)),
                Fail(CasesException.New(CasesText, Symbols, ExList)))
                );
            return Expression.Block(new ParameterExpression[] { ExList }, BlockList);
        }
        public static Expression CreateRepeatedParserEx(BackusExpression Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            var Repeated = (Repeated)Backus;

            var List = Expression.Parameter(typeof(List<PExpression>));
            var Break = Expression.Label();
            var BlockList = new List<Expression>();
            BlockList.Add(Expression.Assign(List, Expression.New(typeof(List<PExpression>))));


            BlockList.Add(
                Expression.Loop(
                Expression.Block(
                Expression.Call(Symbols, "PushState", null),
                CreateIntermediateParserEx(Repeated.Expression, Symbols, Context, ParsersToFill,
                (x) => Expression.Block
                    (
                    Expression.Call(Symbols, "DropState", null),
                    Expression.Call(List, "Add", null, x)
                    ),
                  (x) => Expression.Block
                      (
                        Expression.Call(Symbols, "PopState", null),
                        Expression.Break(Break)
                      ))
                      )
                ));

            var Ret = Expression.New(typeof(CollectionResult).GetConstructor(new Type[] { typeof(IEnumerable<PExpression>), typeof(Guid) }), List, Guid(Backus.ExpressionId));
            BlockList.Add(Expression.Label(Break));
            BlockList.Add(Consume(Ret));

            return Expression.Block(new ParameterExpression[] { List }, BlockList);
        }

        public static Expression CreateOptionalEx(Optional Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            var Cons = typeof(SingleResult).GetConstructor(new Type[] { typeof(PExpression), typeof(Guid) });
            var GID = Guid(Backus.ExpressionId);
            return
                 CreateIntermediateParserEx(Backus.Expression, Symbols, Context, ParsersToFill,
                 (x) => Consume(Expression.New(Cons, x, GID)),
                 (x) => Consume(Expression.New(Cons, Expression.Constant(null, typeof(PExpression)), GID)));
        }

        public static Expression CreateSplitEx(Split Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            var List = Expression.Parameter(typeof(List<PExpression>));
            var BlockList = new List<Expression>();
            var Break = Expression.Label();
            var FailLabel = Expression.Label();

            BlockList.Add(Expression.Assign(List, Expression.New(typeof(List<PExpression>))));
            Func<Expression, Expression> EmptySequenceFail;
            if (Backus.AllowEmptySequences)
                EmptySequenceFail = (x) => Expression.Goto(Break);
            else
                EmptySequenceFail = (x) => Fail(EmptySequencesNotAllowedException.New(Backus.Expression.ToString(), Backus.ToString(), Symbols, x));

            Func<Expression, Expression> UnexpectedTokenFail =
                (x) => Expression.Block
                    (
                    Fail(SequenceException.New(Backus.Expression.ToString() + Backus.Separator.ToString(), Backus.Expression.ToString(), Backus.ToString(), Symbols, x)),
                    Expression.Goto(FailLabel)
                    );

            Func<Expression, Expression> SeparatorSelector;

            if (Backus.Separator.GetType() == typeof(Terminal))
                SeparatorSelector = (x) => x;
            else
                SeparatorSelector = (x) => Expression.Call(List, "Add", null, x);

            BlockList.Add(CreateIntermediateParserEx(Backus.Expression, Symbols, Context, ParsersToFill, (x) => Expression.Call(List, "Add", null, x), EmptySequenceFail));
            BlockList.Add(
                Expression.Loop(
                    Expression.Block(
                        Expression.Call(Symbols, "PushState", null),
                        CreateIntermediateParserEx(Backus.Separator, Symbols, Context, ParsersToFill,
                            (x) => Expression.Block(Expression.Call(Symbols, "DropState", null), SeparatorSelector(x)),
                            (x) => Expression.Block(Expression.Call(Symbols, "PopState", null), Expression.Break(Break))),

                        CreateIntermediateParserEx(Backus.Expression, Symbols, Context, ParsersToFill, (x) => Expression.Call(List, "Add", null, x), UnexpectedTokenFail)
                )));

            var Ret = Expression.New(typeof(CollectionResult).GetConstructor(new Type[] { typeof(IEnumerable<PExpression>), typeof(Guid) }), List, Guid(Backus.ExpressionId));
            BlockList.Add(Expression.Label(Break));
            BlockList.Add(Consume(Ret));
            BlockList.Add(Expression.Label(FailLabel));

            return Expression.Block(new ParameterExpression[] { List }, BlockList);
        }



        /// <summary>
        /// Create an expression tree 
        /// </summary>
        /// <param name="Backus"></param>
        /// <param name="Symbols"></param>
        /// <param name="Context"></param>
        /// <param name="Consume"></param>
        /// <param name="Fail"></param>
        /// <param name="ParsersToFill"></param>
        /// <returns></returns>
        public static Expression CreateIntermediateParserEx(BackusExpression Backus, ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            GuidNames.AddToken(Backus.ExpressionId, Backus.ToString());

            if (Backus is Terminal)
                return CreateTerminaParserEx(Backus, Symbols, Context, ParsersToFill, Consume, Fail);
            else if (Backus is Sequence)
                return CreateSequenceParserEx(Backus, Symbols, Context, ParsersToFill, Consume, Fail);
            else if (Backus is Cases)
                return CreateCasesParserEx(Backus, Symbols, Context, ParsersToFill, Consume, Fail);
            else if (Backus is Repeated)
                return CreateRepeatedParserEx(Backus, Symbols, Context, ParsersToFill, Consume, Fail);
            else if (Backus is Split)
                return CreateSplitEx((Split)Backus, Symbols, Context, ParsersToFill, Consume, Fail);
            else if (Backus is Optional)
                return CreateOptionalEx((Optional)Backus, Symbols, Context, ParsersToFill, Consume, Fail);
            else if (Backus is End)
                return CreateEndParserEx((End)Backus, Symbols, Consume, Fail);
            else if (Backus is Reference)
            {

                return ((Reference)Backus).CreateExpression(Symbols, Context, ParsersToFill, Consume, Fail);
            }
            else
                throw new NotImplementedException();
        }

        public static Expression<ParserDelegate> CreateFinalParser(BackusExpression Backus, Dictionary<Guid, ParserDelegate> ParsersToFill)
        {
            var Symbols = Expression.Parameter(typeof(StateDequeue<ITokenSubstring<Guid>>));
            var Context = Expression.Parameter(typeof(IDictionary<Guid, ParserDelegate>));

            var RetLabel = Expression.Label(typeof(PExpression));
            var If = CreateIntermediateParserEx(Backus, Symbols, Context, ParsersToFill,
                (x) => Expression.Return(RetLabel, x),
                (x) => Expression.Throw(x));

            var Ret =
                Expression.Lambda<ParserDelegate>(
                Expression.Block
                (
                If,
                Expression.Label(RetLabel, Expression.Default(typeof(PExpression)))
                ), Symbols, Context);

            return Ret;
        }

        /// <summary>
        /// Create a parser context that can parse a backus expression
        /// </summary>
        /// <param name="Backus"></param>
        /// <returns></returns>
        public static ParserContext CreateParser(BackusExpression Backus)
        {
            var Symbols = Expression.Parameter(typeof(StateDequeue<ITokenSubstring<Guid>>));
            var Context = Expression.Parameter(typeof(IDictionary<Guid, ParserDelegate>));

            var ParsersToFill = new Dictionary<Guid, ParserDelegate>();

            var RetLabel = Expression.Label(typeof(PExpression));
            var If = CreateIntermediateParserEx(Backus, Symbols, Context, ParsersToFill,
                (x) => Expression.Return(RetLabel, x),
                (x) => Expression.Throw(x));

            var Ret =
                Expression.Lambda<ParserDelegate>(
                Expression.Block
                (
                If,
                Expression.Label(RetLabel, Expression.Default(typeof(PExpression)))
                ), Symbols, Context);

            var Method = Ret.Compile();

            return new ParserContext(ParsersToFill, Method);
        }




        //public static ParserDelegate<TToken, TResult> CreateSequenceParser<TToken, TResult>(Sequence<TResult> Terminal)
        //    where TToken : ISubstring
        //{

        //}

        //public static ParserDelegate<TToken, TResult> CreateGenericParser<TToken, TResult>(BackusExpression<TResult> Expression)
        //    where TToken : ISubstring
        //{
        //    var Type = typeof(Expression);
        //    if (Type.GetGenericTypeDefinition() == typeof(Terminal<,>))
        //        return CreateTerminalParser<TToken, TResult>((Terminal<TToken, TResult>)Expression);
        //    else if (Type == typeof(Sequence<TResult>))
        //    {

        //    }

        //}
    }
}
