using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace GenericCompiler.LexicalAnalysis
{
    /// <summary>
    /// Indicate the name of an operator or keyword guid
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class NamedGuidAttribute : Attribute
    {
        readonly string name;

        // This is a positional argument
        public NamedGuidAttribute(string positionalString)
        {
            this.name = positionalString;
        }

        public string Name
        {
            get { return name; }
        }
    }

    /// <summary>
    /// Common operators and keywords
    /// </summary>
    public static class CommonTokens
    {


        /// <summary>
        /// Aritmetic operators
        /// </summary>
        public class Aritmetic
        {
            //Aritmetic:
            [NamedGuid("+")]
            public static Guid Add = Guid.NewGuid();
            /// <summary>
            /// Minus sign wich can be substaction or negation
            /// </summary>
            [NamedGuid("-")]
            public static Guid Minus = Guid.NewGuid();
            [NamedGuid("*")]
            public static Guid Mul = Guid.NewGuid();
            [NamedGuid("/")]
            public static Guid Div = Guid.NewGuid();
            [NamedGuid("%")]
            public static Guid Modulo = Guid.NewGuid();
        }


        /// <summary>
        /// Assignment operator
        /// </summary>
        public class Assignment
        {
            [NamedGuid("=")]
            public static Guid Assig = Guid.NewGuid();
        }
        /// <summary>
        /// Comparision operators
        /// </summary>
        public class Comparision
        {
            [NamedGuid("==")]
            public static Guid Equals = Guid.NewGuid();
            [NamedGuid("!=")]
            public static Guid NotEquals = Guid.NewGuid();
            [NamedGuid(">")]
            public static Guid GreaterThan = Guid.NewGuid();
            [NamedGuid(">=")]
            public static Guid GreaterOrEq = Guid.NewGuid();
            [NamedGuid("<")]
            public static Guid LessThan = Guid.NewGuid();
            [NamedGuid("<=")]
            public static Guid LessOrEq = Guid.NewGuid();
        }




    }
}
