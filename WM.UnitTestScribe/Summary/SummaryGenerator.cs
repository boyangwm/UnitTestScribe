using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;
using WM.UnitTestScribe.ReportGenerator;
using WM.UnitTestScribe.TestCaseDetector;

namespace WM.UnitTestScribe.Summary {
    public class SummaryGenerator {

        /// <summary> Subject application location </summary>
        public string LocalProj { get; private set; }


        /// <summary> SrcML directory location </summary>
        public string SrcmlLoc { get; private set; }


        //stores all test cases
        private TestCaseDetector.TestCaseDetector _testCaseDetector;


        public  HashSet<TestCaseSummary> AllTestSummary;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localProj"> The subject location</param>
        /// <param name="srcmlloc">Srcml executable file location</param>
        public SummaryGenerator(string localProj, string srcmlloc) {
            this.LocalProj = localProj;
            this.SrcmlLoc = srcmlloc;
            _testCaseDetector = new TestCaseDetector.TestCaseDetector(localProj, srcmlloc);
            _testCaseDetector.AnalysisTestCases();

            AllTestSummary = new HashSet<TestCaseSummary>();
        }


        public void AnalyzeSummary() {
            Console.WriteLine("Hello");
            string dataDir = @"\TESTNAIVE_1.0";
            using (var project = new DataProject<CompleteWorkingSet>(LocalProj, LocalProj, SrcmlLoc))
            {
                project.Update();
                NamespaceDefinition globalNamespace;
                project.WorkingSet.TryObtainReadLock(5000, out globalNamespace);
                try
                {

                    // Step 1.   Build the call graph
                    Console.WriteLine("{0,10:N0} files", project.Data.GetFiles().Count());
                    Console.WriteLine("{0,10:N0} namespaces", globalNamespace.GetDescendants<NamespaceDefinition>().Count());
                    Console.WriteLine("{0,10:N0} types", globalNamespace.GetDescendants<TypeDefinition>().Count());
                    Console.WriteLine("{0,10:N0} methods", globalNamespace.GetDescendants<MethodDefinition>().Count());
                    //Console.Read();
                    var methods = globalNamespace.GetDescendants<MethodDefinition>();
                    int i = 0;
                    // Step 2.   Testing
                    Console.WriteLine("======  test 1 ========= ");
                    foreach (MethodDefinition method in methods)
                    {
                        Console.WriteLine("Method Name : {0}", method.GetFullName());
                        //colect basic ID information
                        var declaringClass = method.GetAncestors<TypeDefinition>().FirstOrDefault();
                        var className = "";
                        if (declaringClass != null)
                        {
                            className = declaringClass.Name;
                        }
                        var nameSpaceName = GetNamespaceByMethod(method);


                        //for testing
                        //if (className != "CPlusPlusCodeParserTests") {
                        //    continue;
                        //}
                        //if (className != "BuiltInTypeFactoryTests") {
                        //    continue;
                        //}

                        //if (i > 10)
                        //{
                        //    continue;
                        //}


                        //if (method.Name != "CreateRequest_ETag")
                        //{
                        //    continue;
                        //}

                        if (IsTestCase(method))
                        {
                           
                            SwumSummary swumSummary = new SwumSummary(method);
                            swumSummary.BasicSummary();
                            TestCaseAnalyzer analyzer = new TestCaseAnalyzer(method);
                            //analyzer.GetTestingObject();
                            string desc = swumSummary.Describe();
                            //writetext.WriteLine(method.Name + "  ,  " + desc);
                            Console.WriteLine(nameSpaceName + "," + className + "," + method.Name + "Swum Description : " + desc);

                            TestCaseSummary tcSummary = new TestCaseSummary(desc, analyzer.ListAssertInfo, method);
                            tcSummary.NameSpaceName = nameSpaceName;
                            tcSummary.ClassName = className;
                            tcSummary.MethodName = method.Name;

                            AllTestSummary.Add(tcSummary);
                            var stmts = method.GetDescendants<Statement>();

                            //delete me
                            //using (StreamWriter sw = File.AppendText(@"D:\d.csv"))
                            //{
                            //   var number = stmts.Count() + 2;
                            //   sw.WriteLine(method.Name + "," + number);
                            //    sw.Close();
                            //}	
                            i++;
                        }
                        

                    }


                }
                finally
                {
                    project.WorkingSet.ReleaseReadLock();
                }


            }


            //Console.ReadLine();
            //string dataDir = @"TESTNAIVE_1.0";
            //using (var project = new DataProject<CompleteWorkingSet>(dataDir, this.LocalProj, this.SrcmlLoc)) {

            //    //Console.WriteLine("============================");
            //    //string unknownLogPath = Path.Combine(project.StoragePath, "unknown.log");
            //    //DateTime start = DateTime.Now, end;
            //    //Console.WriteLine("============================");
            //    //using (var unknownLog = new StreamWriter(unknownLogPath)) {
            //    //    project.UnknownLog = unknownLog;
            //    //    project.UpdateAsync().Wait();

            //    //}
            //    //end = DateTime.Now;
            //    project.Update();
            //    NamespaceDefinition globalNamespace;
            //    project.WorkingSet.TryObtainReadLock(5000, out globalNamespace);
            //    try {

            //        // Step 1.   Build the call graph
            //        Console.WriteLine("{0,10:N0} files", project.Data.GetFiles().Count());
            //        Console.WriteLine("{0,10:N0} namespaces", globalNamespace.GetDescendants<NamespaceDefinition>().Count());
            //        Console.WriteLine("{0,10:N0} types", globalNamespace.GetDescendants<TypeDefinition>().Count());
            //        Console.WriteLine("{0,10:N0} methods", globalNamespace.GetDescendants<MethodDefinition>().Count());
            //        var methods = globalNamespace.GetDescendants<MethodDefinition>();
            //        int i = 0;
            //        StreamWriter writetext = new StreamWriter("write.csv");
            //        foreach (var method in methods) {
            //            //colect basic ID information
            //            var declaringClass = method.GetAncestors<TypeDefinition>().FirstOrDefault();
            //            var className = "";
            //            if (declaringClass != null) {
            //                className = declaringClass.Name;
            //            }
            //            var nameSpaceName = GetNamespaceByMethod(method);


            //            //for testing
            //            //if (className != "CPlusPlusCodeParserTests") {
            //            //    continue;
            //            //}
            //            //if (className != "BuiltInTypeFactoryTests") {
            //            //    continue;
            //            //}
            //            //if (className == "ProgamElementTests" || className == "CPlusPlusCodeParserTests") {
            //            //    continue;
            //            //}

            //            //if (className != "CPlusPlusCodeParserTests") {

            //            //    continue;
            //            //}


            //            //if (className != "SourceLocationTests") {
            //            //    continue;
            //            //}


            //            //if (method.Name != "TestContains_DifferentLines") {
            //            //    continue;
            //            //}


            //            //if (method.Name != "TestSiblingsBeforeSelf_MissingChild") {
            //            //    continue;
            //            //}

            //            //if (i > 30) {
            //            //    continue;
            //            //}

            //            if (IsTestCase(method)) {
            //                i++;
            //                SwumSummary swumSummary = new SwumSummary(method);
            //                swumSummary.BasicSummary();
            //                TestCaseAnalyzer analyzer = new TestCaseAnalyzer(method);
            //                //analyzer.GetTestingObject();
            //                string desc = swumSummary.Describe();
            //                //writetext.WriteLine(method.Name + "  ,  " + desc);
            //                Console.WriteLine(nameSpaceName + "," + className + "," + method.Name + "Swum Description : " + desc);

            //                TestCaseSummary tcSummary = new TestCaseSummary(desc, analyzer.ListAssertInfo, method);
            //                tcSummary.NameSpaceName = nameSpaceName;
            //                tcSummary.ClassName = className;
            //                tcSummary.MethodName = method.Name;

            //                AllTestSummary.Add(tcSummary);
            //                i++;
            //            }
                        

            //        }
            //        writetext.Close();
            //        //Console.WriteLine("total i :" + i);



            //    } finally {
            //        project.WorkingSet.ReleaseReadLock();
            //    }
            //}
        }

        public void GenerateSummary() {
            Console.WriteLine("Now let's generate summary..");
            HomeGenerator homePageGenerator = new HomeGenerator(this.AllTestSummary);
            homePageGenerator.Generate(@"c:\temp\test.html");


        }





        private bool IsTestCase(MethodDefinition md) {
            var declaringClass = md.GetAncestors<TypeDefinition>().FirstOrDefault();
            string nameSpace = GetNamespaceByMethod(md);
           

            if (declaringClass == null || nameSpace == null) {
                return false;
            }
            foreach (var id in this._testCaseDetector.AllTestCases) {
                //Console.WriteLine("namespace id : " + id.NamespaceName + "namespace : " + nameSpace);
                if (id.MethodName == md.Name && id.ClassName == declaringClass.Name && id.NamespaceName == nameSpace) {
                    return true;
                }

            }
            return false;

        }


        private string GetNamespaceByMethod(MethodDefinition md) {
            var allNameSpace = md.GetAncestors<NamespaceDefinition>();
            string nameSpace = "";
            foreach (var ns in allNameSpace) {
                if (ns.Name == "") {
                    continue;
                }
                if (nameSpace == "") {
                    nameSpace += ns.Name;
                } else {
                    nameSpace = ns.Name + "." + nameSpace;
                }

            }
            return nameSpace;
        }

    }
}
