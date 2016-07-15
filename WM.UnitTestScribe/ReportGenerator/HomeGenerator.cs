using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr3.ST;
using WM.UnitTestScribe.Summary;
using System.IO;
using ABB.SrcML.Data;


namespace WM.UnitTestScribe.ReportGenerator {
    public class HomeGenerator {
        public HashSet<TestCaseSummary> AllTestSummary { get; private set; }


        public HomeGenerator(HashSet<TestCaseSummary> allTestSummary) {
            this.AllTestSummary = allTestSummary;
        }



        public void Generate(string path) {
            StringTemplateGroup group = new StringTemplateGroup("myGroup", @".\Templet");
            StringTemplate st = group.GetInstanceOf("Home");
            int ID = 1;
            List<String> allMethodSigniture = new List<string>();
            foreach (var testSummary in AllTestSummary) {
                string methodSignature = testSummary.GetMethodSignature();
                st.SetAttribute("IDNum", ID++);
                st.SetAttribute("MethodSignature", methodSignature);
                var method = testSummary.Method;
                var methodString = GetEntireMethodString(method);
                st.SetAttribute("SourceCode", methodString);
                bool bStartTests = false;
                if (testSummary.SwumSummary.StartsWith("test"))
                {
                    bStartTests = true;
                }
                st.SetAttribute("NotStartTests", !bStartTests);
                st.SetAttribute("SwumDesc", testSummary.SwumSummary.ToLower());
                
                st.SetAttribute("MethodBodyDesc", testSummary.GetBodyDescriptions());
                allMethodSigniture.Add(methodSignature);
            }
            allMethodSigniture.Sort();
            //hyper index
            foreach (var ms in allMethodSigniture) {
                st.SetAttribute("MethodLinkID", ms);

            }


            //st.SetAttribute("Message", "hello ");
            String result = st.ToString(); // yields "int x = 0;"
            //Console.WriteLine(result);

            StreamWriter writetext = new StreamWriter(path);
            writetext.WriteLine(result);
            writetext.Close();

        }


        public string GetEntireMethodString(MethodDefinition method) {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n" + method.ToString() + "{" + "(line " + method.PrimaryLocation.StartingLineNumber + ")" + "\n");
            int methodRelativeLoc = method.PrimaryLocation.StartingColumnNumber;
            var allST = method.GetDescendants<Statement>();
            foreach (var st in allST) {
                for (int i = 0; i <= st.PrimaryLocation.StartingColumnNumber - methodRelativeLoc; i++) {
                    sb.Append(" ");
                }
                //sb.Append(st.ToString().Replace(" . ", ".") + "(line " + st.PrimaryLocation.StartingLineNumber + ")" + "<br>");
                sb.Append(System.Net.WebUtility.HtmlEncode(st.ToString()).Replace(" . ", ".") + "(line " + st.PrimaryLocation.StartingLineNumber + ")" + "\n");

            }
            sb.Append("}");
            return sb.ToString();

        }

    }
}
