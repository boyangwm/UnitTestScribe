using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;
using Antlr3.ST;

namespace WM.UnitTestScribe.Summary {
    public class TestCaseSummary {

        public string SwumSummary { get; private set; }

        public MethodDefinition Method { get; private set; }

        public List<AssertSTInfo> ListAssertInfo { get; private set; }

        public String NameSpaceName;


        public String ClassName;


        public String MethodName;

        public TestCaseSummary(string swumSummary, List<AssertSTInfo> listAssertInfo, MethodDefinition method) {
            if (swumSummary.ToLower().StartsWith("test "))
            {
                swumSummary = "tests " + swumSummary.Substring(5);
            }
            this.SwumSummary = swumSummary;
            this.ListAssertInfo = listAssertInfo;
            this.Method = method;
        }



        public string GetMethodSignature() {
            return NameSpaceName + "." + ClassName + "." + MethodName;
        }


        public string GetBodyDescriptions() {

            StringTemplateGroup group = new StringTemplateGroup("myGroup", @".\Templet");
            StringTemplate st = group.GetInstanceOf("MethodBody");

         




            HashSet<Statement> focalMethodSet = new HashSet<Statement>();




            int num = 1;
            Dictionary<Statement, List<Statement>> focalToAssert = new Dictionary<Statement, List<Statement>>();
            foreach (var assertInfo in ListAssertInfo) {

                if (assertInfo.focalStmt != null) {
                    var fMethod = assertInfo.focalStmt;
                    if (!focalToAssert.ContainsKey(fMethod)) {
                        focalToAssert[fMethod] = new List<Statement>();
                        focalToAssert[fMethod].Add(assertInfo.AssertStatment);
                    } else {
                        focalToAssert[fMethod].Add(assertInfo.AssertStatment);
                    }

                    focalMethodSet.Add(assertInfo.focalStmt);
                }
            }


            //Focal method part
            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<Statement, List<Statement>> entry in focalToAssert) {
                var fMethod = entry.Key;
                if (first) {
                    sb.Append(
                        "<br><br><span class=\"glyphicon glyphicon-bell\"></span><b>This unit test case includes following focal methods: </b>");
                    first = false;
                }
                StringBuilder focalInfo = new StringBuilder();
                if (IsAssert(fMethod)) {
                    focalInfo.Append("(" + num++ + ") " + "An assertion in line ");
                    var firstOrDefault = fMethod.Locations.FirstOrDefault();
                    int line = 0;
                    if (firstOrDefault != null) {
                        line = firstOrDefault.StartingLineNumber;
                    }
                    focalInfo.Append(line);
                    focalInfo.Append("  validating that ");
                    focalInfo.Append(AssertSTInfo.TranslateAssert(fMethod));
                    sb.Append("<br>" + focalInfo);
                } else {
                    var firstOrDefault = fMethod.Locations.FirstOrDefault();
                    if (firstOrDefault != null) {
                        focalInfo.Append("(" + num++ + ") " + fMethod.ToString().Replace(" . ", ".") + "(@line " +
                                    firstOrDefault.StartingLineNumber + ")");
                    } else {
                        focalInfo.Append("(" + num++ + ") " + fMethod.ToString().Replace(" . ", "."));
                    }
                    var assertSet = entry.Value;
                    int i = 0;
                    foreach (var assertStmt in assertSet) {
                        if (i == 0) {
                            focalInfo.Append("<br> This focal method is related with assertions at ");
                        } else {
                            focalInfo.Append(", ");
                        }
                        StringTemplate stFA = group.GetInstanceOf("FocalAssert");
                        stFA.SetAttribute("Statment", System.Net.WebUtility.HtmlEncode(assertStmt.ToString().Replace(" . ", ".")));
                        firstOrDefault = assertStmt.Locations.FirstOrDefault();
                        int line = 0;
                        if (firstOrDefault != null) {
                            line = firstOrDefault.StartingLineNumber;
                        }
                        stFA.SetAttribute("LineNumber", line);
                        focalInfo.Append(stFA.ToString());
                        i++;
                    }
                    sb.Append("<br>" + focalInfo);
                }

            }
            st.SetAttribute("FocalMethodDesc", sb.ToString());


            //Assertion part
            int ID = 1;
            StringBuilder TestCasesDescBuilder = new StringBuilder();
            foreach (var assertInfo in ListAssertInfo) {
                //avoid since the information is shown in focal method discription
                if (focalMethodSet.Contains(assertInfo.AssertStatment)) {
                    continue;
                }
                //string methodSignature = testSummary.GetMethodSignature();
                string discAssertion = assertInfo.GetAssertionDescriptions(ID++);

                //for focal method
                //Statement focalStmt = assertInfo.FindThisFocalMethodStmt();
                //
                TestCasesDescBuilder.Append(discAssertion);
            }

            st.SetAttribute("TotalNumGZ", ID > 1);
            if (ID != 1)
                st.SetAttribute("TestCasesDesc", TestCasesDescBuilder.ToString());
            //st.SetAttribute("FocalMethodDesc", "Focal Method");


            return st.ToString();


        }

        private bool IsAssert(Statement st) {
            var exp = st.Content;
            if (exp != null) {
                //check if it's an assertion
                var subExps = exp.Components.ToList();
                if (subExps.Count >= 3) {
                    var nameUse = subExps.ElementAt(0) as NameUse;
                    if (nameUse != null && nameUse.Name == "Assert") {
                        return true;
                    }
                }

            }
            return false;
        }


    }


}
