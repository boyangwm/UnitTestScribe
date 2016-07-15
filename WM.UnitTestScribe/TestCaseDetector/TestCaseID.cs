using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.UnitTestScribe.TestCaseDetector {
    public class TestCaseID {

        public string NamespaceName { get ; private set; }    //Indicates the namespace
        
        
        public string ClassName { get; private set; }       //Indicates the class name
        
        
        public string MethodName { get; private set; }      //Indicates the method name


        /// <summary>
        /// Constructor
        /// </summary>
        public TestCaseID(string nameSpaceName, string className, string methodName) {
            this.NamespaceName = nameSpaceName;
            this.ClassName = className;
            this.MethodName = methodName;
        }

    }
}
