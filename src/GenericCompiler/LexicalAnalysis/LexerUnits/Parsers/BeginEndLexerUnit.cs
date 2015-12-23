using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.LexicalAnalysis.LexerUnits.Parsers
{
    /// <summary>
    /// Parse lexer units that are enclosed between two other lexer units, such as comments
    /// </summary>
    public class BeginEndLexerUnit : INoLookupLexerUnitParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="AllowOpenEnds">If true when the begin is found, this would be considered a valid unit, event if the end is never found</param>
        /// <param name="Id"></param>
        public BeginEndLexerUnit(INoLookupLexerUnitParser Begin, INoLookupLexerUnitParser End, bool AllowOpenEnds, string friendlyName)
        {
            this.friendlyName = friendlyName;

            this.AllowOpenEnds = AllowOpenEnds;
            this.Begin = Begin;
            this.End = End;

            Reset();
        }


        public readonly bool AllowOpenEnds;
        private INoLookupLexerUnitParser Begin;
        private INoLookupLexerUnitParser End;

        public enum BeginEndState
        {
            BeginExpected,
            EndOrMiddleExpected,
            EndExpected,
            Invalid
        }

        public BeginEndState State;
        public void Append(char Current)
        {
            switch (State)
            {
                case BeginEndState.BeginExpected:
                    {
                        Begin.Append(Current);
                        if (Begin.CurrentValidity == LexerUnitValidity.Invalid)
                        {
                            Begin.Reset();
                            Begin.Append(Current);
                        }

                        if (Begin.CurrentValidity == LexerUnitValidity.Valid || Begin.CurrentValidity == LexerUnitValidity.ValidPosible)
                        {
                            CurrentValidity = AllowOpenEnds ? LexerUnitValidity.ValidPosible : LexerUnitValidity.Posible;
                            State = BeginEndState.EndOrMiddleExpected;
                        }
                        else
                            CurrentValidity = Begin.CurrentValidity;
                        break;
                    }
                case BeginEndState.EndOrMiddleExpected:
                    {
                        End.Append(Current);
                        if (End.CurrentValidity == LexerUnitValidity.Invalid)
                        {
                            End.Reset();
                            End.Append(Current);
                        }

                        if (End.CurrentValidity == LexerUnitValidity.Valid)
                        {
                            State = BeginEndState.Invalid;
                            CurrentValidity = LexerUnitValidity.Valid;
                        }
                        else if (End.CurrentValidity == LexerUnitValidity.ValidPosible)
                        {
                            CurrentValidity = LexerUnitValidity.ValidPosible;
                            State = BeginEndState.EndExpected;
                        }
                        else if (End.CurrentValidity == LexerUnitValidity.Invalid)
                        {
                            End.Reset();
                        }
                        break;
                    }
                case BeginEndState.EndExpected:
                    {
                        End.Append(Current);
                        if (End.CurrentValidity == LexerUnitValidity.Valid)
                        {
                            State = BeginEndState.Invalid;
                            CurrentValidity = LexerUnitValidity.Valid;
                        }
                        else if (End.CurrentValidity == LexerUnitValidity.ValidPosible)
                        {
                            //Do nothing
                        }
                        else
                        {
                            Reset();
                            CurrentValidity = LexerUnitValidity.Invalid;
                            State = BeginEndState.Invalid;
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException("State '" + State.ToString() + "' not implemented");

            }
        }

        public void Reset()
        {
            this.Begin.Reset();
            this.End.Reset();
            State = BeginEndState.BeginExpected;
            CurrentValidity = LexerUnitValidity.Posible;
        }

        public LexerUnitValidity CurrentValidity
        {
            get;
            private set;
        }

        private string friendlyName;
        public override string ToString()
        {
            return friendlyName;
        }
    }
}
