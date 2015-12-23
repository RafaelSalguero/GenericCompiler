using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.ParserResult
{
    /// <summary>
    /// A resulting expression from a perser, each Backus expression defining constraints of parsings have a related PExpression wich is the result of parsing that expression
    /// </summary>
    /// <typeparam name="TTerminal"></typeparam>
    public abstract class PExpression
    {
        public PExpression(Guid ExpressionId)
        {
            this.ExpressionId = ExpressionId;
        }

        /// <summary>
        /// The ID of the identified bakus naur expression
        /// </summary>
        public Guid ExpressionId { get; private set; }

        public string ExpressionFriendlyName
        {
            get
            {
                return GuidNames.GetName(ExpressionId);
            }
        }
    }
}
