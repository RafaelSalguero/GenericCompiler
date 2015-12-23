using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.SyntaxTree.Expressions
{
    /// <summary>
    /// An expression that holds an unsolved name wich can be a variable, method, type...
    /// </summary>
    public class NameExpression : Expression
    {
        public NameExpression(ISubstring Name)
            : base(Name)
        { }

        public string Name
        {
            get
            {
                return this.Substring();
            }
        }
    }
}
