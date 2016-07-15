using WM.UnitTestScribe.Stereotype.Taxonomy;
using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    public abstract class MethodRule {

        /// <summary>
        /// Creates a new MethodRule using default values for data sets.
        /// </summary>
        public MethodRule()
            : base() {
            InitializeMembers();
        }

        /// <summary>
        /// Determines whether the supplied AnalyzedElement matches the conditions of this rule.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool RuleMatchedClass(MethodAnalyzer mAnalyzer) {
            return MakeClassification(mAnalyzer);
        }

        /// <summary>
        /// Performs further rule testing beyond the RuleMatchedClass method. 
        /// RuleMatchedClass tests whether the node is a MethodDeclarationNode, then calls this method.
        /// </summary>
        /// <param name="node">The MethodAnalyzer to test.</param>
        /// <returns>True if the node matches this rule, False otherwise.</returns>
        protected abstract bool MakeClassification(MethodAnalyzer mAnalyzer);

        /// <summary>
        /// Returns the method stereotype
        /// </summary>
        /// <returns></returns>
        public abstract MethodStereotype GetMethodStereotype();

        /// <summary>
        /// Returns the number of changed data member in the given method.
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int NumOfChangedField(MethodAnalyzer mAnalyzer) {
            return mAnalyzer.SetSelfFields.Count;
        }

        /// <summary>
        /// Returns the number of directly returned data member in the given method.
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int NumOfDirectRetField(MethodAnalyzer mAnalyzer) {
            return mAnalyzer.GetSelfFields.Count;
        }

        /// <summary>
        /// Returns the number of indirectly returned data member in the given method.
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int NumOfIndirectRetField(MethodAnalyzer mAnalyzer) {
            return mAnalyzer.PropertyFields.Count;
        }

      

        /// <summary>
        /// Returns number of non primitive parameters in this method
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int GetNumOfNonPrimitiveParameters(MethodAnalyzer mAnalyzer) {
            int count = 0;
            foreach(var para in mAnalyzer.ParametersInfo) {
                if(!mAnalyzer.IsPrimitiveType(para.GetVariableType())) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Returns number of non primitive local variables in this method
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int GetNumOfNonPrimitiveVariables(MethodAnalyzer mAnalyzer) {
            int count = 0;
            foreach(var vb in mAnalyzer.VariablesInfo) {
                if(!mAnalyzer.IsPrimitiveType(vb.GetVariableType())) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets number of returned variables which has field assignment
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int GetNumOfRetFieldVar(MethodAnalyzer mAnalyzer) {
            int count = 0;
            foreach(var vb in mAnalyzer.VariablesInfo) {
                if(vb.IsReturned && vb.AssignedFields.Count != 0) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets number of returned parameters which has field assignment
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int GetNumOfRetFieldPara(MethodAnalyzer mAnalyzer) {
            int count = 0;
            foreach(var vb in mAnalyzer.ParametersInfo) {
                if(vb.IsReturned && vb.AssignedFields.Count != 0) {
                    count++;
                }
            }
            return count;
        }


        /// <summary>
        /// Gets number of modified non-primitive parameters
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        protected int GetNumOfModifiedObjPara(MethodAnalyzer mAnalyzer) {
            int count = 0;
            foreach(var vb in mAnalyzer.ParametersInfo) {
                if(!mAnalyzer.IsPrimitiveType(vb.GetVariableType()) && vb.IsModified) {
                    count++;
                }
            }
            return count;
        }


        /// <summary>
        /// Initializes members (For extention)
        /// </summary>
        private void InitializeMembers() {

        }
    }
}
