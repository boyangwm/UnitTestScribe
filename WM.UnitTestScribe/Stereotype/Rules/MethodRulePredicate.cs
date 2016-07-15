using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. No data member is changed.
    /// 2. Return type is bool. 
    /// 3. Do not directly return any data member.
    /// return (1 && 2 && 3)
    /// </summary>
    public class MethodRulePredicate :MethodRule {
        /// <summary>
        /// Creates a new PrdicateMethodRule using default values for data sets.
        /// </summary>
        public MethodRulePredicate() : base() { }


        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            // 1. No data member is changed.
            if(this.NumOfChangedField(mAnalyzer) != 0) {
                return false;
            }
            // 2. Return type is bool. 
            if(mAnalyzer.RTypeIsBoolean()) {
                // 3. Do not directly return any data member.
                if(this.NumOfDirectRetField(mAnalyzer) == 0) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns the method stereotype
        /// </summary>
        /// <returns></returns>
        public override MethodStereotype GetMethodStereotype() {
            return MethodStereotype.Predicate;
        }
    }
}
