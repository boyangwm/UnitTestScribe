using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. if the method is a constructor
    /// return 1 
    /// </summary>
    public class MethodRuleConstructor :MethodRule {

        /// <summary>
        /// Creates a new ConstructorMethodRole using default values for data sets.
        /// </summary>
        public MethodRuleConstructor() : base() { }

        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            /// 1. if the method is a constructor
            if(mAnalyzer.IsConstructor == true) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns the method stereotype
        /// </summary>
        /// <returns></returns>
        public override MethodStereotype GetMethodStereotype() {
            return MethodStereotype.Constructor;
        }
    }
}
