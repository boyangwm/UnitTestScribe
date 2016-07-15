using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. The method has no statment. (should not be abstract)
    /// return 1 
    /// </summary>
    public class MethodRuleEmpty :MethodRule {
        /// <summary>
        /// Creates a new EmptyMethodRule using default values for data sets.
        /// </summary>
        public MethodRuleEmpty() : base() { }

        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            //1. The method has no statment. (should not be abstract)
            if(mAnalyzer.HasStatements == false) {
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
            return MethodStereotype.Empty;
        }
    }
}
