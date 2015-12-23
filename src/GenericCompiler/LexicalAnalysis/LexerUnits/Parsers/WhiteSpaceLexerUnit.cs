using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.LexicalAnalysis.LexerUnits.Parsers
{
    /// <summary>
    /// Identify a sequence of spaces and tabs
    /// </summary>
    public class WhiteSpaceLexerUnit : INoLookupLexerUnitParser
    {
        public enum SpaceLexerState
        {
            /// <summary>
            /// Waiting for a space or tab
            /// </summary>
            SpaceOrTab,

            Invalid
        }

        public WhiteSpaceLexerUnit()
        {
            Reset();
        }

        public SpaceLexerState State;

        public void Append(char Current)
        {
            switch (State)
            {
                case SpaceLexerState.SpaceOrTab:
                    if (Current == ' ')
                    {
                        State = SpaceLexerState.SpaceOrTab;
                        CurrentValidity = LexerUnitValidity.ValidPosible;
                    } 
                    else if (Current == '\t')
                    {
                        State = SpaceLexerState.SpaceOrTab;
                        CurrentValidity = LexerUnitValidity.ValidPosible;
                    }
                    else
                    {
                        State = SpaceLexerState.Invalid;
                        CurrentValidity = LexerUnitValidity.Invalid;
                    }
                    break;
                default:
                    throw new NotSupportedException("State '" + State.ToString() + "' not supported by the SpaceLexer unit");
            }
        }

        public void Reset()
        {
            CurrentValidity = LexerUnitValidity.Posible;
            State = SpaceLexerState.SpaceOrTab;
        }

        public LexerUnitValidity CurrentValidity
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return "space";
        }
    }
}
