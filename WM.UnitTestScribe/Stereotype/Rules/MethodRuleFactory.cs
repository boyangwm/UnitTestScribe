using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {
 
    /// <summary>
    /// Rules:
    /// 1. Cannot be primitive type
    /// 2. Local varible is instantiated and returned.  
    /// 3. Create and return a new object directly
    /// return (1 && (2||3) )
    /// </summary>
    public class MethodRuleFactory :MethodRule {
        /// <summary>
        /// Creates a new FactoryMethodRule using default values for data sets.
        /// </summary>
        public MethodRuleFactory() : base() { }


        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer mAnalyzer) {
            // 1. Cannot be primitive type
            if(mAnalyzer.IsPrimitiveType(mAnalyzer.ReturnType)) {
                return false;
            }

            // 2. Local varible is instantiated and returned.  
            foreach(var vi in mAnalyzer.VariablesInfo) {
                if(vi.IsReturned == true && vi.IsInstantiated) {
                    return true;
                }
            }
            foreach(var vi in mAnalyzer.ParametersInfo) {
                if(vi.IsReturned == true && vi.IsInstantiated) {
                    return true;
                }
            }

            // 3. Create and return a new object directly
            if(mAnalyzer.IsReturnNewObj) {
                return true;
            }

            //// *4. Parameter is instantiated in the method and it's not primitive type.  
            //foreach(var vi in aMethod.ParametersInfo) {
            //    if(!aMethod.IsPrimitiveType(vi.GetVariableType()) &&  vi.IsInstantiated) {
            //        return true;
            //    }
            //}

            // returns false if it could not pass all the cases. 
            return false;
        }


        /// <summary>
        /// Returns the method stereotype
        /// </summary>
        /// <returns></returns>
        public override MethodStereotype GetMethodStereotype() {
            return MethodStereotype.Factory;
        }

    }
}
