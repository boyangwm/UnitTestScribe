using WM.UnitTestScribe.Stereotype.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Collaborator - is a method that works on objects of classes different from itself
    /// Rules:
    /// 1. At least one of the method's parameters or local variables is an object
    /// 2. Invokes external method(s)
    /// Returns (1 || 2)
    /// </summary>
    public class MethodRuleCollaborator :MethodRule {

        /// <summary>
        /// Creates a new CollaboratorMethodRule using default values for data sets.
        /// </summary>
        public MethodRuleCollaborator() : base() { }

        /// <summary>
        /// Classifies the given method  
        /// </summary>
        /// <param name="node">The AnalyzedMethod to test</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodAnalyzer aMethod) {
            int numOfNonPrimitiveP = this.GetNumOfNonPrimitiveParameters(aMethod);
            int numOfNonPrimitiveV = this.GetNumOfNonPrimitiveVariables(aMethod);
            //int numOfRetFieldVP = this.GetNumOfRetFieldPara(aMethod) + this.GetNumOfRetFieldVar(aMethod);
            //int numOfModifiedObjPara = this.GetNumOfModifiedObjPara(aMethod);

            // 1. At least one of the method's parameters or local variables is an object
            // Collaborator - is a method that works on objects of classes different from itself
            if(numOfNonPrimitiveP > 0 || numOfNonPrimitiveV > 0) {
                return true;
            }

            // 2.ChangeScribe implemented "invokes external method AND local method"  (otherwise, it could be a controller)
            if(aMethod.InvokedExternalMethods.Count != 0 && aMethod.InvokedLocalMethods.Count != 0) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the method stereotype
        /// </summary>
        /// <returns></returns>
        public override MethodStereotype GetMethodStereotype() {
            return MethodStereotype.Collaborator;
        }
    }
}
