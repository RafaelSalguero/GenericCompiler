using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler
{
    interface ICompiler<TCode>
    {
        IEnumerable<TCode> Compile(string Code);
    }
}
