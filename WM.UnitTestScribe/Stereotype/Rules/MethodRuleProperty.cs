using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. No data member is changed.
    /// 2. Return type is NOT bool and NOT void. 
    /// 3. Do not return one data member directly. (directly > 1 || indirectly > 0)
    /// return (1 && 2 && 3)
    /// </summary>
    public class MethodRuleProperty :MethodRule {

        /// <summary>
        /// Creates a new PropertyMethodRule using default values for data sets.
        /// </summary>
        public MethodRuleProperty() : base() { }


        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            //1. No data member is changed.
            if(this.NumOfChangedField(mAnalyzer) != 0) {
                return false;
            }
            //2. Return type is NOT bool. 
            if(mAnalyzer.RTypeIsBoolean() || mAnalyzer.RTypeIsVoid()) {
                return false;
            }
            // 3. Do not return one data member directly. (directly > 1 || indirectly > 0)
            if(this.NumOfDirectRetField(mAnalyzer) > 1 || this.NumOfIndirectRetField(mAnalyzer) > 0) {
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
            return MethodStereotype.Property;
        }
    }
}
