using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCompiler.PatternMatching.Permutations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericCompilerUnitTest
{
    [TestClass]
    public class Permutations
    {
        [TestMethod]
        public void PowerCombine()
        {
            var digits = new int[][]
            {
                new int []{1,2,3},
                new int [] {4, 5, 6}
            };

            var r = PermutationGenerator.PowerCombine(digits);

            var result = new int[][]
            {
                new int [] {1, 4},
                new int [] {2, 4},
                new int [] {3, 4},

                new int [] {1, 5},
                new int [] {2, 5},
                new int [] {3, 5},

                new int [] {1, 6},
                new int [] {2, 6},
                new int [] {3, 6}
            };

        }
    }
}
