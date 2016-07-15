
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WM.UnitTestScribe.Stereotype.Taxonomy;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. Only one data member is changed
    /// 2. Return type is void or Boolean
    /// return (1 && 2)
    /// </summary>
    public class MethodRuleSet : MethodRule{

        /// <summary>
        /// Creates a new GetMRule using default values for data sets.
        /// </summary>
        public MethodRuleSet() : base() { }


        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            int num = this.NumOfChangedField(mAnalyzer);
            //1. Only one data member is changed
            if(num == 1) {
                //2. Return type is void or Boolean
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
            return MethodStereotype.Set;
        }
    }
}
