using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.CompilerStages.OperatorSolver
{
    public interface IArgPosOperator
    {
        OperatorArgumentPosition ArgumentPosition { get; }
    }

 
    /// <summary>
    /// An operator with precedence
    /// </summary>
    public interface IPrecedence
    {
        int Precedence { get; }
    }

    /// <summary>
    /// Identify a token as a parenthesis
    /// </summary>
    public interface IParenthesis
    {
        bool IsCloedParenthesis { get; }
        bool IsOpenParenthesis { get; }
    }
    /// <summary>
    /// Identify a token as a comma
    /// </summary>
    public interface IComma
    {
        bool IsComma { get; }
    }
}
