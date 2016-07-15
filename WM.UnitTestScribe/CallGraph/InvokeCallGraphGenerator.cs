using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;

namespace WM.UnitTestScribe.CallGraph {
    class InvokeCallGraphGenerator {
        /// <summary> Subject application location </summary>
        public string LocalProj;

        /// <summary> SrcML directory location </summary>
        public string SrcmlLoc;

        public InvokeCallGraphGenerator(string localProj, string srcmlloc) {
            this.LocalProj = localProj;
            this.SrcmlLoc = srcmlloc;
        }


        public void run() {
            Console.Out.WriteLine("Invoke call graph generator ");


            string dataDir = @"TESTNAIVE_1.0";
            //string proPath = @"C:\Users\boyang.li@us.abb.com\Documents\RunningTest\Input\ConsoleApplication1";
            //string proPath = @"C:\Users\boyang.li@us.abb.com\Documents\RunningTest\Input\SrcML\ABB.SrcML";
            using (var project = new DataProject<CompleteWorkingSet>(dataDir, this.LocalProj, this.SrcmlLoc)) {

                Console.WriteLine("============================");
                string unknownLogPath = Path.Combine(project.StoragePath, "unknown.log");
                DateTime start = DateTime.Now, end;
                Console.WriteLine("============================");
                using (var unknownLog = new StreamWriter(unknownLogPath)) {
                    project.UnknownLog = unknownLog;
                    project.UpdateAsync().Wait();

                }
                end = DateTime.Now;

                NamespaceDefinition globalNamespace;
                project.WorkingSet.TryObtainReadLock(5000, out globalNamespace);
                try {

                    // Step 1.   Build the call graph
                    Console.WriteLine("{0,10:N0} files", project.Data.GetFiles().Count());
                    Console.WriteLine("{0,10:N0} namespaces", globalNamespace.GetDescendants<NamespaceDefinition>().Count());
                    Console.WriteLine("{0,10:N0} types", globalNamespace.GetDescendants<TypeDefinition>().Count());
                    Console.WriteLine("{0,10:N0} methods", globalNamespace.GetDescendants<MethodDefinition>().Count());
                    var methods = globalNamespace.GetDescendants<MethodDefinition>();

                    CGManager cgm = new CGManager();
                    cgm.BuildCallGraph(methods);

                    // Step 2.   Testing
                    Console.WriteLine("======  test 1 ========= ");
                    foreach (MethodDefinition m in methods) {
                        Console.WriteLine("Method Name : {0}", m.GetFullName());
                        List<List<MethodDefinition>> paths = cgm.FindCalleeList(m);
                        foreach (List<MethodDefinition> path in paths) {
                            foreach (MethodDefinition mc in path) {

                                Console.Write("{0}--->", mc.Name);

                            }
                            Console.WriteLine("");
                        }

                    }


                    Console.WriteLine("======  test 2 ========= ");
                    List<List<MethodDefinition>> paths2 = cgm.FindCalleeListByName("ConsoleApplication1.Class1.m1");
                    foreach (List<MethodDefinition> path in paths2) {
                        foreach (MethodDefinition mc in path) {

                            Console.Write("{0}--->", mc.Name);

                        }
                        Console.WriteLine("");
                    }


                    Console.WriteLine("======  test 3 ========= ");
                    int sum = 0;
                    int pathNum = 0;
                    foreach (MethodDefinition m in methods) {
                        Console.WriteLine("Method Name : {0}", m.GetFullName());
                        List<List<MethodDefinition>> paths = cgm.FindCallerList(m);
                        foreach (List<MethodDefinition> path in paths) {

                            sum += path.Count;
                            pathNum++;
                            foreach (MethodDefinition mc in path) {
                                Console.Write("{0}<---", mc.Name);
                            }
                            Console.WriteLine("");
                        }
                    }
                    Console.WriteLine("average level : " + (double)sum / pathNum);


                } finally {
                    project.WorkingSet.ReleaseReadLock();
                }


            }


            Console.ReadLine();


        }

    }
}
