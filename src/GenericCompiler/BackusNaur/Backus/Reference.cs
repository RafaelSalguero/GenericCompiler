using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// A backus expression that holds a reference to another expression. Used for allowing recursive definitions
    /// </summary>
    public class Reference : BackusExpression
    {
        public Reference()
        {

        }


        private BackusExpression expression = null;
        /// <summary>
        /// The referenced expression
        /// </summary>
        public BackusExpression Value
        {
            get
            {
                if (expression == null)
                    throw new ArgumentException("The reference expression hasn't been assigned yet");
                else
                    return expression;
            }
            set
            {
                if (expression == null)
                    expression = value;
                else
                    throw new ArgumentException("Refernce expressions can be assigned only once");
            }
        }

        public Expression CreateExpression(ParameterExpression Symbols, ParameterExpression Context, Dictionary<Guid, RecursiveDescentParser.ParserDelegate> ParsersToFill, Func<Expression, Expression> Consume, Func<Expression, Expression> Fail)
        {
            if (!ParsersToFill.ContainsKey(Value.ExpressionId))
            {
                ParsersToFill.Add(Value.ExpressionId, null);

                var ValueEx = RecursiveDescentParser.CreateFinalParser(Value, ParsersToFill);
                ParsersToFill[Value.ExpressionId] = ValueEx.Compile();
            }

            //Get The parser delegate method:
            Expression Method = Expression.Property(Context, "Item", RecursiveDescentParser.Guid(Value.ExpressionId));
            //Call the parser delegate from the context:
            Expression Call = Expression.Call(Method, "Invoke", null, Symbols, Context);

            var exception = Expression.Parameter(typeof(ParserException), "ex");

            return Expression.TryCatch(Consume(Call),
                Expression.Catch(exception, Fail(exception)));
        }

        protected override string InternalToString()
        {
            return "[recursive]";
        }
    }
}
