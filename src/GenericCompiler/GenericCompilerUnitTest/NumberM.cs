using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.TextSide;
using GenericCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenericCompiler.CompilerStages.TextSide;
using GenericCompiler.CompilerStages.MultiGroping;
using GenericCompiler.CompilerStages.OperatorSolver;
using System.Diagnostics;

namespace GenericCompilerUnitTest
{
    [TestClass]
    public class NumberM
    {
        enum PrimitiveOp
        {
            None,
            OpenPar,
            ClosedPar,
            Comma,
            Add,
            Sub,
            Mul,
            Div,
            Neg,
        }

        [DebuggerDisplay("{ToString}")]
        private struct Operator : IArgPosOperator, IParenthesis, IComma, IOriginalToken<string>
        {
            public Operator(OperatorArgumentPosition argPos, PrimitiveOp Op, string token)
            {
                this.argPos = argPos;
                this.Op = Op;
                this.token = token;
            }

            public readonly PrimitiveOp Op;
            private readonly string token;
            private OperatorArgumentPosition argPos;
            public OperatorArgumentPosition ArgumentPosition
            {
                get { return argPos; }
            }

            public bool IsComma
            {
                get { return Op == PrimitiveOp.Comma; }
            }

            public bool IsCloedParenthesis
            {
                get { return Op == PrimitiveOp.ClosedPar; }
            }

            public bool IsOpenParenthesis
            {
                get { return Op == PrimitiveOp.OpenPar; }
            }

            public string OriginalToken
            {
                get { return token; }
            }

            public override string ToString()
            {
                return Op.ToString();
            }
        }

        [TestMethod]
        public void NumberWordSplit()
        {
            var seps = new string[] { " ", "+", "-", "*", "/", ".", "(", ")", ",", "!" };
            string s = "(2 !), --2";

            var operators = new Operator[]
            {
                   new  Operator ( OperatorArgumentPosition.None  , PrimitiveOp.OpenPar  , "("),
                   new  Operator ( OperatorArgumentPosition.None  , PrimitiveOp.ClosedPar   , ")"),
                   new  Operator ( OperatorArgumentPosition.None  , PrimitiveOp.Comma    , ","),
                new  Operator ( OperatorArgumentPosition.Binary , PrimitiveOp.Add , "+"),
                new  Operator ( OperatorArgumentPosition.Binary , PrimitiveOp.Sub , "-"),
                new  Operator ( OperatorArgumentPosition.PrefixUnary  , PrimitiveOp.Neg , "-"),
                new  Operator ( OperatorArgumentPosition.PostfixUnary  , PrimitiveOp.Factorial  , "!"),
                new  Operator ( OperatorArgumentPosition.Binary   , PrimitiveOp.BinaryExclamation   , "!"),
            };

            var split = s.AsSubstring().SplitWords(seps).Select((x) => x.Substring()).Where((x) => x != " ").ToArray();
            var ops = OperatorSolver.Solve2(split, operators).ToArray();

        }
    }
}
