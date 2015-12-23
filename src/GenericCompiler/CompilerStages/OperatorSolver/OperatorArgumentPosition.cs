using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.CompilerStages.OperatorSolver
{
    public enum OperatorArgumentPosition
    {
        None,
        Binary,
        /// <summary>
        /// Operators that are before the operand, such as -x, +y
        /// </summary>
        PrefixUnary,

        /// <summary>
        /// Operator that are after the operand, such as x!, 
        /// </summary>
        PostfixUnary,
        /// <summary>
        /// When the operator is enclosed between parenthesis such as (+)
        /// </summary>
        FunctionAlias
    }
}
