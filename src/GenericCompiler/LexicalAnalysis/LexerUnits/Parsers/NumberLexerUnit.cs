using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.LexicalAnalysis.LexerUnits.Parsers
{
    /// <summary>
    /// Parse decimal numbers. All decimal numbers share the same id
    /// </summary>
    public class NumberLexerUnit : INoLookupLexerUnitParser
    {
        public NumberLexerUnit() : this(false) { }

        public NumberLexerUnit(bool AllowSign)
        {
            this.AllowSign = AllowSign;
            Reset();
        }


        /// <summary>
        /// True to allow numbers with sign indicator
        /// </summary>
        public bool AllowSign
        {
            get;
            set;
        }

        public enum NumberState
        {
            /// <summary>
            /// This is an invalid number
            /// </summary>
            Invalid,

            /// <summary>
            /// A digit or a sign expected
            /// </summary>
            DigitOrSignOrDot,

            /// <summary>
            /// A digit or decimal dot expected
            /// </summary>
            DigitOrDot,

            /// <summary>
            /// Decimal digits expected
            /// </summary>
            DecimalDigits
        }

        private NumberState state;
        public NumberState State
        {
            get
            {
                return state;
            }
        }
        public void Append(char Current)
        {
            switch (state)
            {
                case NumberState.DigitOrSignOrDot:
                    {
                        if (AllowSign && Current == '+' || Current == '-')
                        {
                            state = NumberState.DigitOrDot;
                            CurrentValidity = LexerUnitValidity.Posible;
                        }
                        else if (char.IsDigit(Current))
                        {
                            state = NumberState.DigitOrDot;
                            CurrentValidity = LexerUnitValidity.ValidPosible;
                        }
                        else if (Current == '.')
                        {
                            state = NumberState.DecimalDigits;
                        }
                        else
                        {
                            state = NumberState.Invalid;
                            CurrentValidity = LexerUnitValidity.Invalid;
                        }
                        break;
                    }
                case NumberState.DigitOrDot:
                    {
                        if (Current == '.')
                        {
                            state = NumberState.DecimalDigits;
                            CurrentValidity = LexerUnitValidity.Posible;
                        }
                        else if (char.IsDigit(Current))
                        {
                            state = NumberState.DigitOrDot;
                            CurrentValidity = LexerUnitValidity.ValidPosible;
                        }
                        else
                        {
                            state = NumberState.Invalid;
                            CurrentValidity = LexerUnitValidity.Invalid;
                        }
                        break;
                    }
                case NumberState.DecimalDigits:
                    {
                        if (char.IsDigit(Current))
                        {
                            state = NumberState.DecimalDigits;
                            CurrentValidity = LexerUnitValidity.ValidPosible;
                        }
                        else
                        {
                            state = NumberState.Invalid;
                            CurrentValidity = LexerUnitValidity.Invalid;
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException("State '" + state.ToString() + "' not implemented");
            }
        }

        public void Reset()
        {
            state = NumberState.DigitOrSignOrDot;
            CurrentValidity = LexerUnitValidity.Posible;
        }

        public LexerUnitValidity CurrentValidity
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return "number";
        }
    }
}
