using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Rules {

    /// <summary>
    /// Rule collector for CSharp language
    /// </summary>
    public class CSharpMethodRuleCollector :MethodRuleCollector {
        public CSharpMethodRuleCollector()
            : base() {
        }

        /// <summary>
        /// Initializes the rule list to a defined set of Rule objects.
        /// </summary>
        protected override void DefineRuleSet() {
            var ruleList = new List<MethodRule>();
            ruleList.Add(new MethodRuleConstructor());
            ruleList.Add(new MethodRuleEmpty());
            ruleList.Add(new MethodRuleGet());
            ruleList.Add(new MethodRulePredicate());
            ruleList.Add(new MethodRuleProperty());
            ruleList.Add(new MethodRuleSet());
            ruleList.Add(new MethodRuleCommand());
            ruleList.Add(new MethodRuleFactory());
            ruleList.Add(new MethodRuleCollaborator());
            ruleList.Add(new MethodRuleController());
            ruleList.Add(new MethodRuleLocalController());
            this.Rules = ruleList;
        }
    }
}
