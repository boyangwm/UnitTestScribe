using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. No class field is changed.
    /// 2. Return type is not void. 
    /// 3. Only return one class field. (1 Directly && 0 Indirectly)
    /// return (1 && 2 && 3)
    /// </summary>
    public class MethodRuleGet :MethodRule {
        /// <summary>
        /// Creates a new GetMRule using default values for data sets.
        /// </summary>
        public MethodRuleGet() : base() { }


        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            // 1. No class field is changed.
            if(this.NumOfChangedField(mAnalyzer) != 0) {
                return false;
            }
            // 2. Return type is not void. 
            if(mAnalyzer.RTypeIsVoid()) {
                return false;
            }
            // 3. Only return one class field. (Directly)
            if(this.NumOfDirectRetField(mAnalyzer) == 1 && this.NumOfIndirectRetField(mAnalyzer) == 0) {
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
            return MethodStereotype.Get;
        }

    }
}
