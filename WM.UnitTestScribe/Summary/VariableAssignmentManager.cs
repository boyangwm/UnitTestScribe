using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;

namespace WM.UnitTestScribe.Summary {
    public class VariableAssignmentManager {
        public Dictionary<VariableDeclaration, List<AssignmentPath>> VarDictionary;

        public VariableAssignmentManager() {
            this.VarDictionary = new Dictionary<VariableDeclaration, List<AssignmentPath>>();
        }



        public void AddNewRelations(VariableDeclaration assignedVar, HashSet<VariableDeclaration> assigningVars, Statement stmt) {


            //InitValidStates
            if (!VarDictionary.ContainsKey(assignedVar)) {
                AssignmentPath aPath = new AssignmentPath();
                aPath.AddNewPair(assignedVar, assignedVar.ParentStatement);
                List<AssignmentPath> listPath = new List<AssignmentPath>();
                listPath.Add(aPath);
                VarDictionary[assignedVar] = listPath;
            }
            foreach (var assigningVar in assigningVars) {
                if (!VarDictionary.ContainsKey(assigningVar)) {
                    AssignmentPath aPath = new AssignmentPath();
                    aPath.AddNewPair(assigningVar, assigningVar.ParentStatement);
                    List<AssignmentPath> listPath = new List<AssignmentPath>();
                    listPath.Add(aPath);
                    VarDictionary[assigningVar] = listPath;
                }
            }
            List<AssignmentPath> newListPath = new List<AssignmentPath>();

            //Then update
            foreach (var assigningVar in assigningVars) {
                var ListPath = VarDictionary[assigningVar];
                foreach (var path in ListPath) {
                    var newPath = path.Copy();
                    //newPath.AddNewPair(assigningVar, stmt);
                    newPath.AddNewPair(assignedVar, stmt);
                    newListPath.Add(newPath);
                }
            }
            VarDictionary[assignedVar] = newListPath;

        }

        public List<AssignmentPath> GetPathByVariable(VariableDeclaration vd) {
            if (VarDictionary.ContainsKey(vd)) {
                return VarDictionary[vd];
            } else {
                return new List<AssignmentPath>();
            }
        }




        public VariableDeclaration FindMostComplecatedVD() {
            int max = 0;
            VariableDeclaration vd = null;
            foreach (KeyValuePair <VariableDeclaration, List<AssignmentPath> >entry in VarDictionary) {
                int current = 0;
                foreach (var path in entry.Value) {
                    current += path.ReturnSizes();
                }
                if (current > max) {
                    vd = entry.Key;
                    max = current;
                }
                
            }
            return vd;

        }

    }
}
