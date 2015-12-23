using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler
{
    public class CompilerException : Exception
    {
        public CompilerException(string Message, ISubstring Position)
            : base(Message)
        {
            this.Position = Position;
        }
        public readonly ISubstring Position;
    }


}
