using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur.Backus
{
    /// <summary>
    /// Throws an exception if the end of file was not reached
    /// </summary>
    public class End : BackusExpression
    {
        protected override string InternalToString()
        {
            return ".";
        }
    }
}
