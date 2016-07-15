
using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML;
using WM.UnitTestScribe.Stereotype.Rules;
using WM.UnitTestScribe.Stereotype.Taxonomy;

namespace WM.UnitTestScribe.Stereotype {
    public class StereotypedMethod {
        /// <summary>
        /// The method SrcML.net
        /// </summary>
        private readonly MethodDefinition _method;

        /// <summary>
        /// the given method rule collector which stores the rules. 
        /// </summary>
        private readonly MethodRuleCollector _ruleCollector;

        /// <summary>
        /// The method analyzer used to analyze the code
        /// </summary>
        public MethodAnalyzer Analyzer { get; private set; }


        /// <summary>
        /// The list of the stereotypes which are matched for the given method.
        /// </summary>
        public List<MethodStereotype> ListMatchedStereotypes { get; private set; }


        /// <summary>
        /// Use predefined rule collector to analyze the method <param name="method"></param>
        /// </summary>
        /// <param name="method"></param>
        public StereotypedMethod(MethodDefinition method) {
            this._method = method;
            if(this._method.ProgrammingLanguage == Language.CSharp) {
                this._ruleCollector = new CSharpMethodRuleCollector();
            }
            this.Analyze();
        }

        /// <summary>
        /// Gets the method name
        /// </summary>
        /// <returns></returns>
        public string GetMethodName() {
            return this.Analyzer.Method.Name;
        }


        /// <summary>
        /// Get the class name
        /// </summary>
        /// <returns></returns>
        public string GetClassName() {
            return this.Analyzer.DeclaringClass.Name;
        }


        /// <summary>
        /// Get the return type
        /// </summary>
        /// <returns></returns>
        public string GetReturnType() {
            if(Analyzer.ReturnType == null) {
                return "void";
            } else {
                return this.Analyzer.ReturnType.ToString();
            }
        }


        /// <summary>
        /// Checks return type is bool or not
        /// </summary>
        /// <returns></returns>
        public bool RTypeIsBoolean() {
            return this.Analyzer.RTypeIsBoolean();
        }


        /// <summary>
        /// Checks return type is void or not
        /// </summary>
        /// <returns></returns>
        public bool RTypeIsVoid() {
            return this.Analyzer.RTypeIsVoid();
        }


        /// <summary>
        /// Checks if the method is a constructor
        /// </summary>
        /// <returns></returns>
        public bool IsConstructor() {
            return this.Analyzer.IsConstructor;
        }



        /// <summary>
        /// Checks if the method is a IsDestructor
        /// </summary>
        /// <returns></returns>
        public bool IsDestructor() {
            return this.Analyzer.IsDestructor;
        }



        /// <summary>
        /// Checks if the returned type of the method is primitive
        /// </summary>
        /// <returns></returns>
        public bool IsRetPrimitiveType() {
            return this.Analyzer.IsPrimitiveType(Analyzer.ReturnType);
        }


        /// <summary>
        /// returns Parameters
        /// </summary>
        public IEnumerable<VariableDeclaration> GetParas() {
            return this.Analyzer.Paras;
        }


        /// <summary>
        /// Returns the first type in the list
        /// </summary>
        /// <returns></returns>
        public MethodStereotype GetPrimaryType() {
            return ListMatchedStereotypes.FirstOrDefault();
        }

        /// <summary>
        /// Applies the rules to the method
        /// </summary>
        public void Analyze() {
            if(this._method.ProgrammingLanguage == Language.CSharp) {
                this.Analyzer = new CSharpMethodAnalyzer(_method);
                ListMatchedStereotypes = _ruleCollector.ApplyRules(Analyzer);
            }
        }
    }
}
