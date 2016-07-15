using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.Swum;
using ABB.Swum.Nodes;
using Antlr3.ST;
using CommandLine.Text;
using WM.UnitTestScribe.CallGraph;
using WM.UnitTestScribe.Summary;
using WM.UnitTestScribe.TestCaseDetector;
using ABB.SrcML.Data;


namespace WM.UnitTestScribe {
    public class Program {

        /// <summary> Subject application location </summary>
        //public static readonly string LocalProj = @"D:\Research\myTestSubjects\Callgraph\CallgraphSubject";
        //public static readonly string LocalProj = @"D:\Research\Subjects\google-api-dotnet-client-master";
        //public static readonly string LocalProj = @"D:\Research\Subjects\Sando-master";
        //public static readonly string LocalProj = @"D:\Research\Subjects\SrcML.NET";
        public static readonly string LocalProj = @"D:\Research\Subjects\Glimpse-master";
        /// <summary> SrcML directory location </summary>
        public static readonly string SrcmlLoc = @"D:\Research\SrcML\";


        /// <summary>
        /// Command line testing
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            DateTime dt = DateTime.Now;
           //args = new string[] { "hello" };
           // args = new string[] { "testcases", "--loc", LocalProj, "--srcmlPath", SrcmlLoc }; 
           args = new string[] { "summary", "--loc", LocalProj, "--srcmlPath", SrcmlLoc }; 
            var options = new Options();
            string invokedVerb = null;
            object invokedVerbOptions = null;


            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, verbOptions) => {
                    invokedVerb = verb;
                    invokedVerbOptions = verbOptions;
                })) {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
            if (invokedVerb == "callgraph") {
                var callGraphOp = (CallgraphOptions)invokedVerbOptions;
                var generator = new InvokeCallGraphGenerator(callGraphOp.LocationsPath, callGraphOp.SrcMLPath);
                generator.run();
            } else if (invokedVerb == "testcases") {
                var testCaseOp = (TestCaseDetectOptions)invokedVerbOptions;
                var detector = new TestCaseDetector.TestCaseDetector(testCaseOp.LocationsPath, testCaseOp.SrcMLPath);
                detector.AnalysisTestCases();
                Console.WriteLine("print testcases");
                foreach (var testCaseId in detector.AllTestCases) {
                    Console.WriteLine(testCaseId.NamespaceName + "  "+ testCaseId.ClassName + "  " + testCaseId.MethodName);
                }
            } else if (invokedVerb == "summary") {
                var SummaryOp = (SummarizeTestOptions)invokedVerbOptions;
                var summary = new SummaryGenerator(SummaryOp.LocationsPath, SummaryOp.SrcMLPath);
                Console.WriteLine("This is summary");
                summary.AnalyzeSummary();
                summary.GenerateSummary();
                Console.WriteLine("Done!!!!!!  Thanks.");

            } else if (invokedVerb == "hello") {

                Console.WriteLine("Hello");
                string dataDir = @"\TESTNAIVE_1.0";
                //string proPath = @"C:\Users\boyang.li@us.abb.com\Documents\RunningTest\Input\ConsoleApplication1";
                //string proPath = @"C:\Users\boyang.li@us.abb.com\Documents\RunningTest\Input\SrcML\ABB.SrcML";
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
                        Console.ReadLine();
                        var methods = globalNamespace.GetDescendants<MethodDefinition>();

                        // Step 2.   Testing
                        Console.WriteLine("======  test 1 ========= ");
                        foreach (MethodDefinition m in methods)
                        {
                            Console.WriteLine("Method Name : {0}", m.GetFullName());

                        }
                       
                    }
                    finally
                    {
                        project.WorkingSet.ReleaseReadLock();
                    }


                }
              

                Console.ReadLine();


                //StringTemplateGroup group = new StringTemplateGroup("myGroup", @".\Templet");
                //StringTemplate st = group.GetInstanceOf("FocalAssert");
                //st.SetAttribute("Statment", "st1");
                //st.SetAttribute("LineNumber", "line 1 ");

                //st.SetAttribute("Statment", "st2");
                //st.SetAttribute("LineNumber", "line 2 ");

                //st.SetAttribute("Message", "hihi ");
                //String result = st.ToString(); // yields "int x = 0;"
                //Console.WriteLine(result);

                //=======================
                //StringTemplate hello = new StringTemplate("Hello, $name$");
                //hello.SetAttribute("name", "World");
                //Console.Out.WriteLine(hello.ToString());

                //StringTemplateGroup group = new StringTemplateGroup("myGroup", @"C:\Tutorials");
                //StringTemplate helloAgain = group.GetInstanceOf("homepage");

                //helloAgain.SetAttribute("title", "Welcome To StringTemplate");
                //helloAgain.SetAttribute("name", "World");
                //helloAgain.SetAttribute("friends", "Terence");
                //helloAgain.SetAttribute("friends", "Kunle");
                //helloAgain.SetAttribute("friends", "Micheal");
                //helloAgain.SetAttribute("friends", "Marq");


                //Console.Out.WriteLine(helloAgain.ToString());

                //============================================================

                //StringTemplateGroup group = new StringTemplateGroup("myGroup", @"c:\Tutorials");
                //StringTemplate query = group.GetInstanceOf("SqlParameters");

                //query.SetAttribute("ID", "SqlDataSource1");
                //query.SetAttribute("ID", "SqlDataSource1");

                //query.SetAttribute("Prefixes", "Cookie");
                //query.SetAttribute("ParamNames", "City");
                //query.SetAttribute("Types", "string");
                //query.SetAttribute("SourceNames", "CookieName");
                //query.SetAttribute("SourceIds", "cookieCity");

                //query.SetAttribute("Prefixes", "");
                //query.SetAttribute("ParamNames", "Country");
                //query.SetAttribute("Types", "string");
                //query.SetAttribute("SourceNames", false);
                //query.SetAttribute("SourceIds", "");

                //query.SetAttribute("Prefixes", "QueryString");
                //query.SetAttribute("ParamNames", "Address");
                //query.SetAttribute("Types", "string");
                //query.SetAttribute("SourceNames", "QueryStringField");
                //query.SetAttribute("SourceIds", "qsAddr");


                //query.SetAttribute("Prefixes", "Boyang");
                //query.SetAttribute("ParamNames", "BoyangTest");
                //query.SetAttribute("Types", "int");
                //query.SetAttribute("SourceNames", "");
                ////query.SetAttribute("SourceIds", "qsAddr");

                //query.SetAttribute("Prefixes", "Boyang2");
                //query.SetAttribute("ParamNames", "2222");
                //query.SetAttribute("Types", "int");
                ////query.SetAttribute("SourceNames", "");


                //Console.WriteLine(query.ToString());

                Console.WriteLine("print hello");

            }
            TimeSpan ts = DateTime.Now - dt;
            Console.WriteLine(ts.ToString());
            Console.ReadLine();
        }


        private class Options {
            [VerbOption("callgraph", HelpText = "Analyze stereotype of methods in the project")]
            public CallgraphOptions StereotypeVerb { get; set; }

            [VerbOption("testcases", HelpText = "find all test cases in a project")]
            public TestCaseDetectOptions FindAllTestCaseVerb { get; set; }


            [VerbOption("summary", HelpText = "summarize test cases in a project")]
            public SummarizeTestOptions SummaryTestCaseVerb { get; set; }


            [VerbOption("hello", HelpText = "Print hello for testing")]
            public HelloOptions HelloVerb { get; set; }

            [HelpVerbOption]
            public string GetUsage(string verb) {
                return HelpText.AutoBuild(this, verb);
            }
        }




        /// <summary>
        /// Stereotype detector
        /// </summary>
        private class CallgraphOptions {
            /// <summary> Subject application location </summary>
            [Option("loc", Required = true, HelpText = "The subject project folder")]
            public string LocationsPath { get; set; }

            [Option("srcmlPath", Required = true, HelpText = "The path to Srcml.exe")]
            public string SrcMLPath { get; set; }
        }



        /// <summary>
        /// Options for findAllTestCases
        /// </summary>
        private class TestCaseDetectOptions {
            /// <summary> Subject application location </summary>
            [Option("loc", Required = true, HelpText = "The subject project folder")]
            public string LocationsPath { get; set; }

            [Option("srcmlPath", Required = true, HelpText = "The path to Srcml.exe")]
            public string SrcMLPath { get; set; }
        }



        /// <summary>
        /// Stereotype detector
        /// </summary>
        private class SummarizeTestOptions {
            /// <summary> Subject application location </summary>
            [Option("loc", Required = true, HelpText = "The subject project folder")]
            public string LocationsPath { get; set; }

            [Option("srcmlPath", Required = true, HelpText = "The path to Srcml.exe")]
            public string SrcMLPath { get; set; }
        }



        /// <summary>
        /// print hello for testing 
        /// </summary>
        private class HelloOptions {
        }
    }

}
