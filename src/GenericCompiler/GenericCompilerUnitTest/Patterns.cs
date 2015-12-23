using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.PatternMatching.Patterns;
using GenericCompiler.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenericCompiler;
namespace GenericCompilerUnitTest
{
    [TestClass]
    public class Patterns
    {
        [TestMethod]
        public void Sequence()
        {
            var Seq = new IPattern<string, string>[] 
              {
                  PatternFactory.Any <string,string> ().AsNamed ("x"),
                  PatternFactory.Any <string,string> ().AsNamed ("x"),
              };

            var Pat = new SequencePattern<string, string>("+", Seq, false, true, false, true, TreeExtensions.CreateLeaf("0"));
            var Tokens = TreeExtensions.CreateSequence(new string[] { "1", "1", "1", "1" }, "+");

            var Match = Pat.Match(Tokens).ToArray();
        }

        [TestMethod]
        public void CodelineMatching()
        {
            string[] separators = new string[] { "=", "(", ")", "+", "-", "*", "/", ";", " ", "\n", "," };

            Func<string, ITree<string>> tokenize = (code) =>
                TreeExtensions.CreateSequence
                (
                    WordSplitter.SplitWords(code, separators).Select((x) => x.ToString()).Where((x) => x != " " && x != "\n")
                );

            var assignLine = tokenize("x = (1 + 2) * 3");
            var declarationLine = tokenize("int x");
            var declarationLineIncomplete = tokenize("int x = ");
            var declarationLineWithDefault = tokenize("int x = 10 + 3");


            var assignPattern = new IPattern<string, string>[]
            {
               PatternFactory.Named <string,string>("var").Leaf (), 
               PatternFactory.Literal <string,string> ("="),
               PatternFactory.Named <string,string> ("value")
            }.AsSequencePattern();

            var variablePattern = new IPattern<string, string>[]
            {
                PatternFactory .Named <string ,string>("type").Leaf (), 
                PatternFactory .Named <string ,string>("name").Leaf (), 
                new IPattern <string ,string >[] 
                {
                    PatternFactory .Literal <string ,string >("="),
                    PatternFactory .Named <string,string> ("value")
                }.AsSequencePattern ().AsOptional()
            }.AsSequencePattern();


            //  var AssignMatch = assignPattern.Match(assignLine).SingleOrDefault();

            // var VarMatch = variablePattern.Match(declarationLineWithDefault).SingleOrDefault();

            var argListLine = tokenize("(int edad, float hola, float z)");
            var ArgListPat = new IPattern<string, string>[]
            {
                PatternFactory .Literal <string,string>("("),
                new IPattern <string,string>[]
                {
                    PatternFactory.Leaf  <string,string>(),
                    PatternFactory .Leaf <string,string>()
                }.AsSequencePattern ().All ().Split (","),
                PatternFactory .Literal <string,string>(")")
            }.AsSequencePattern().AsNamed("args");

            var argSep = ArgListPat.Match(argListLine).ToArray();

        }
    }
}
