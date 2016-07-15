using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.UnitTestScribe.Stereotype {
    public class VariableInfo {


        public VariableDeclaration Variable { get; set; }             // the veriable declaration
        public bool Initialized { get; set; }                                // record the variable is init or not.
        public bool IsReturned { get; set; }                                    // the variable is returned in the method
        public bool IsModified { get; set; }                                    // the variable has been modified

        public HashSet<VariableDeclaration> AssignedFields;                     // the fields have been assigned to this variable
        public bool IsInstantiated { get; set; }                                // the variable is isInstantiated in the method
        public bool IsFieldChange { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="vd"></param>
        public VariableInfo(VariableDeclaration vd) {
            IsReturned = false;
            IsModified = false;
            AssignedFields = new HashSet<VariableDeclaration>();

            this.Variable = vd;
            if(vd.Initializer != null) {
                Initialized = true;
            }
        }

        /// <summary>
        /// Get the name of the variable 
        /// </summary>
        /// <returns></returns>
        public String GetName() {
            return this.Variable.Name;
        }

        /// <summary>
        /// Get the type of the variable
        /// </summary>
        /// <returns></returns>
        public TypeUse GetVariableType() {
            return this.Variable.VariableType;
        }
    }
}
