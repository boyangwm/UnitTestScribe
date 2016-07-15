using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;
using Antlr3.ST;
using Microsoft.SqlServer.Server;

namespace WM.UnitTestScribe.Summary {
    public class AssignmentPath {
        //public VariableDeclaration Variable { get; private set; }

        public List<VariableAndStmt> VariableStmts { get; private set; }

        public AssignmentPath() {//VariableDeclaration variable) {
            //this.Variable = variable;
            VariableStmts = new List<VariableAndStmt>();
        }


        public void AddNewPair(VariableDeclaration variable, Statement stmt) {
            VariableStmts.Add(new VariableAndStmt(variable, stmt));

        }


        public void AddNewPair(VariableAndStmt vs) {
            VariableStmts.Add(vs);

        }


        /// <summary>
        /// Copies the current AssignmentPath and return the copy
        /// </summary>
        /// <returns></returns>
        public AssignmentPath Copy() {
            AssignmentPath newPath = new AssignmentPath();
            foreach (var vs in VariableStmts) {
                newPath.AddNewPair(vs);
            }
            return newPath;
        }


        public string GetPathDescriptions(int num) {
            StringTemplateGroup group = new StringTemplateGroup("myGroup", @".\Templet");
            StringTemplate st = group.GetInstanceOf("PathTemplate");

            st.SetAttribute("Number", num);
            int i = 0;
            bool isFirst = true;
            foreach (var vs in VariableStmts) {
                
                var loc = vs.Stmt.Locations.FirstOrDefault();
                string lineNumber = "";
                if (loc != null) {

                    if (vs.Variable.Location.SourceFileName == loc.SourceFileName) {
                        lineNumber += loc.StartingLineNumber;
                    }

                }

                if (isFirst) {
                    if (vs.Variable.Name == "")
                        return "";
                    st.SetAttribute("rootVar", vs.Variable.Name);
                    isFirst = false;
                }
                st.SetAttribute("Variable", vs.Variable.Name + "(@line " + lineNumber + ")");
                //st.SetAttribute("Variable", System.Net.WebUtility.HtmlEncode(vs.Stmt.ToString().Replace(" . ", ".")) + "(@line " + lineNumber + ")" );
                i++;
            }
            return ("\n"+ st.ToString()).Trim();

        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            for (int i = VariableStmts.Count - 1; i >= 0; i--) {
                var vs = VariableStmts.ElementAt(i);
                builder.Append(vs.Variable.Name);
                if (i != 0) {
                    builder.Append(" <-- ");
                }
            }
            return builder.ToString();
        }

        public int ReturnSizes() {
            return VariableStmts.Count;
        }
    }
}
