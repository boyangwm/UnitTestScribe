using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {


    /// <summary>
    /// Rules:
    /// 1. Complex change to the object state is performed. (More than one class field is changed.)
    /// 2. Return type is void or Boolean
    /// return (1 && 2)
    /// </summary>
    public class MethodRuleCommand :MethodRule {

        /// <summary>
        /// Creates a new CommandMethodRule using default values for data sets.
        /// </summary>
        public MethodRuleCommand() : base() { }

        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            int num = this.NumOfChangedField(mAnalyzer);
            // 1. Complex change to the object state is performed. (More than one class field is changed.)
            if(num > 1) {
                // 2. Return type is void or Boolean
                if(mAnalyzer.RTypeIsBoolean() || mAnalyzer.RTypeIsVoid()) {
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
            return MethodStereotype.Command;
        }
    }
}
