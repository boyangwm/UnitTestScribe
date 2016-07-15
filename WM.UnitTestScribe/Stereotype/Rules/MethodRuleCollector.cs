
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WM.UnitTestScribe.Stereotype.Taxonomy;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// A method rule collector
    /// </summary>
    public class MethodRuleCollector {
        /// <summary>
        /// The set of rules.
        /// </summary>
        public List<MethodRule> Rules {get; protected set;}

        /// <summary>
        /// Creates a new MethodRule with the default rule set.
        /// </summary>
        //The rule set is not actually initialized until ApplyRules is called.
        //This is so that child classes can define additional properties as necessary for their rules, and these
        //can be set prior to the rule set being initialized.
        public MethodRuleCollector() : base() { }

        /// <summary>
        /// Creates a new RuleCollector with the specified rule set.
        /// </summary>
        /// <param name="ruleSet">The set of rules</param>
        public MethodRuleCollector(List<MethodRule> ruleSet)
            : base() {
            this.Rules = ruleSet;
        }



        /// <summary>
        /// Initializes the list of rules.  virtual
        /// </summary>
        protected virtual void DefineRuleSet() {
            this.Rules = new List<MethodRule>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mAnalyzer"></param>
        /// <returns></returns>
        public List<MethodStereotype> ApplyRules(MethodAnalyzer mAnalyzer) {
            List<MethodStereotype> matchedStereotypes = new List<MethodStereotype>();
            if(Rules == null) { DefineRuleSet(); }
            foreach(MethodRule rule in this.Rules) {
                if(rule.RuleMatchedClass(mAnalyzer)) {
                    matchedStereotypes.Add(rule.GetMethodStereotype());
                }
            }
            if(matchedStereotypes.Count == 0) {
                matchedStereotypes.Add(MethodStereotype.Unclassified);
            }
            return matchedStereotypes;
        }
    }
}
