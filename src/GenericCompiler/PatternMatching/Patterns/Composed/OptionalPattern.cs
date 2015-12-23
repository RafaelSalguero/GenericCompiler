using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.PatternMatching.Patterns.Composed
{
    /// <summary>
    /// A pattern that passes if the given pattern pass or if the subject is an empty sequence
    /// </summary>
    public class OptionalPattern<TKey, TValue> : CasesPattern<TKey, TValue>
    {
        public OptionalPattern(IPattern<TKey, TValue> Pattern) :
            base(new IPattern<TKey, TValue>[] { PatternFactory.Empty<TKey, TValue>(), Pattern })
        { desc = "<" + Pattern.ToString() + ">"; }

        private readonly string desc;
        public override string ToString()
        {
            return desc;
        }
    }
}
