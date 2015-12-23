using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    public abstract class BackusExpression
    {
        public BackusExpression()
        {
            ExpressionId = Guid.NewGuid();
        }

        /// <summary>
        /// The identifier of this expression
        /// </summary>
        public Guid ExpressionId
        {
            get;
            private set;
        }

        /// <summary>
        /// Set a given name and id to this expression id
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Id"></param>
        public BackusExpression AsNamed(string Name)
        {
            GuidNames.AddToken(ExpressionId, Name);
            return this;
        }



        public sealed override string ToString()
        {
            if (GuidNames.HasName(ExpressionId))
                return GuidNames.GetName(ExpressionId);
            else
                return InternalToString();
        }

        protected abstract string InternalToString();
    }
}
