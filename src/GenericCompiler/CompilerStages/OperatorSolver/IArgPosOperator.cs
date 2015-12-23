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

    public interface IIsOperator
    {
        bool IsOperator { get; }
    }

    public interface IOperator<TOperator>
    {
        TOperator Operator { get; }
    }
    /// <summary>
    /// Identify a token as a parenthesis
    /// </summary>
    public interface IIsParenthesis
    {
        bool IsClosedParenthesis { get; }
        bool IsOpenParenthesis { get; }
    }
    /// <summary>
    /// Identify a token as a comma
    /// </summary>
    public interface IIsComma
    {
        bool IsComma { get; }
    }


}
