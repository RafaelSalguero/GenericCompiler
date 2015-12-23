using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.ParserResult
{
    /// <summary>
    /// Result of selecting one expression (Cases or optional).
    /// The Value is null if an optional expression didn't return in a value
    /// </summary>
    public class SingleResult : PExpression
    {
        public SingleResult(PExpression Value, Guid ExpressionId)
            : base(ExpressionId)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Return true if Value == null
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Value == null;
            }
        }

        /// <summary>
        /// The matched value, can be null
        /// </summary>
        public PExpression Value
        {
            get;
            private set;
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return "<[empty]>";
            }
            else
                return "<" + Value.ToString() + ">";
        }
    }
}
