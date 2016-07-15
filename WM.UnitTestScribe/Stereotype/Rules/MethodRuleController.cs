using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rules:
    /// 1. None of the method's parameters and local variables is an object
    /// 2. Invokes external method but no local infomation has been changed. 
    /// return (1 && 2)
    /// </summary>
    public class MethodRuleController :MethodRule {

        /// <summary>
        /// Creates a new ControllerMethodRule using default values for data sets.
        /// </summary>
        public MethodRuleController() : base() { }

        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            int numOfNonPrimitiveP = this.GetNumOfNonPrimitiveParameters(mAnalyzer);
            int numOfNonPrimitiveV = this.GetNumOfNonPrimitiveVariables(mAnalyzer);

            // 1. None of the method's parameters and local variables is an object
            if(numOfNonPrimitiveP == 0 && numOfNonPrimitiveV == 0) {
                //2. Invokes external method but no local infomation has been changed. 
                if(mAnalyzer.InvokedExternalMethods.Count > 0 && mAnalyzer.InvokedLocalMethods.Count == 0 &&
                    mAnalyzer.GetSelfFields.Count == 0 && mAnalyzer.PropertyFields.Count == 0 && mAnalyzer.SetSelfFields.Count == 0) {
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
            return MethodStereotype.Controller;
        }
    }
}
