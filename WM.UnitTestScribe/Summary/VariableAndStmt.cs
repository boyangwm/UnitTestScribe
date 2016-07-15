using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.UnitTestScribe.Summary {
    public class VariableAndStmt {
        public VariableDeclaration Variable { get; private set; }
        public Statement Stmt { get; private set; }



        public VariableAndStmt(VariableDeclaration vd, Statement stmt) {
            this.Variable = vd;
            this.Stmt = stmt;
        }
    }
}
