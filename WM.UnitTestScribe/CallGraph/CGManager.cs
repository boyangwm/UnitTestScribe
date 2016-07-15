using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;

namespace WM.UnitTestScribe.CallGraph {
    public class CGManager {
        /// <summary>
        /// level bound
        /// </summary>
        readonly int LEVELTHRESHOLD = 5;

        /// <summary>call graph </summary>
        CallGraph cg = new CallGraph();

        /// <summary> record the maximum call level for the method  </summary>
        private readonly Dictionary<MethodDefinition, int> levelMap = new Dictionary<MethodDefinition, int>();

        /// <summary> A dictionary for finding a method by the signiture </summary>
        readonly Dictionary<String, MethodDefinition> methodDictionary = new Dictionary<String, MethodDefinition>();


        private List<List<MethodDefinition>> _calleeList;
        private List<List<MethodDefinition>> _callerList;


        /// <summary>
        /// build the call graph based on all methods in the project
        /// </summary>
        /// <param name="methods"></param>
        public void BuildCallGraph(IEnumerable<MethodDefinition> methods) {
            //Do we have a method which can find a MethodDefinition by the 
            //method signiture? (Just implemented an Dictionary)
            foreach (MethodDefinition method in methods) {
                methodDictionary[method.GetFullName()] = method;
                //add the method.
                cg.AddNode(method);
            }

            Console.WriteLine("build call graph");

            foreach (MethodDefinition method in methods) {
                //Console.WriteLine("method  {0}", method.GetFullName());
                var mdCalls = from statments in method.GetDescendantsAndSelf()
                              from expression in statments.GetExpressions()
                              from call in expression.GetDescendantsAndSelf<MethodCall>()
                              select call;
                //Console.WriteLine("calls count : {0}" , mdCalls.Count());
                foreach (MethodCall call in mdCalls) {
                    MethodDefinition calleeM = FindMatchedMd(call);
                    if (calleeM == null) { continue; }
                    if (!cg.ContainsMethod(calleeM) || !cg.ContainsMethod(method)) {
                        Console.WriteLine("ERROR: Did not find the callee in the call graph");
                        throw new Exception("Did not find the callee");
                    }
                    cg.AddCalleeEdge(method, calleeM);
                    cg.AddCallerEdge(method, calleeM);
                }

            }
            //cg.cgPrint();
        }


        /// <summary>
        /// Find MethodDefinition matching from MethodCall
        /// </summary>
        /// <param name="mc"></param>
        /// <returns></returns>
        private MethodDefinition FindMatchedMd(MethodCall mc) {
            INamedEntity match = null;
            try {
                match = mc.FindMatches().FirstOrDefault();
            } catch (Exception e) {
                Console.WriteLine("{0}:{1}:{2}: Call Exception {3}", mc.Location.SourceFileName, mc.Location.StartingLineNumber, mc.Location.StartingColumnNumber, e);
            }
            if (null != match) {
                //Console.WriteLine("match : {0} ", match);
                if (match is MethodDefinition) {
                    //Console.WriteLine("method Definition");
                    MethodDefinition md = (MethodDefinition)match;
                    //Console.WriteLine("md full name :" + md.GetFullName());
                    return md;
                }
            }
            return null;
        }


        /// <summary>
        /// return function by the full name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MethodDefinition GetMethodByFullName(string key) {
            MethodDefinition returnMethod = null;
            if (!methodDictionary.TryGetValue(key, out returnMethod)) {
                return new MethodDefinition();
            } else {
                return returnMethod;
            }
        }


        /// <summary>
        /// find all callee paths from m to all its reachable paths (bounded by a constant) 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public List<List<MethodDefinition>> FindCalleeList(MethodDefinition m) {
            _calleeList = new List<List<MethodDefinition>>();
            if (!cg.ContainsMethod(m)) {
                return _calleeList;
            }
            List<MethodDefinition> path = new List<MethodDefinition>();
            FindCalleeListHelper(m, new List<MethodDefinition>(path), 0);

            return _calleeList;
        }



        /// <summary>
        /// The helper function for findCalleeList
        /// </summary>
        /// <param name="m"></param>
        /// <param name="path"></param>
        /// <param name="level"></param>
        private void FindCalleeListHelper(MethodDefinition m, List<MethodDefinition> path, int level) {
            if (m == null) {
                _calleeList.Add(path);
                return;
            }


            if (level > this.LEVELTHRESHOLD) {
                _calleeList.Add(path);
                return;
            }
            else {
                path.Add(m);
            }


            HashSet<MethodDefinition> curCalleeList = cg.ReturnCallee(m);
            if (curCalleeList.Count == 0) {
                _calleeList.Add(path);
                return;
            }
            foreach (MethodDefinition calleeM in curCalleeList) {
                if (level > this.LEVELTHRESHOLD - 1)
                    return;
                FindCalleeListHelper(calleeM, new List<MethodDefinition>(path), level + 1);
            }
        }



        /// <summary>
        /// return all callee paths from m to all its reachable paths by function name. 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public List<List<MethodDefinition>> FindCalleeListByName(String m) {
            MethodDefinition currentM = GetMethodByFullName(m);
            return FindCalleeList(currentM);
        }




        /// <summary>
        /// find all caller paths from m to all its reachable paths (bounded by a constant) 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public List<List<MethodDefinition>> FindCallerList(MethodDefinition m) {
            _callerList = new List<List<MethodDefinition>>();
            if (!cg.ContainsMethod(m)) {
                return _callerList;
            }
            List<MethodDefinition> path = new List<MethodDefinition>();
            FindCallerListHelper(m, new List<MethodDefinition>(path), 0);

            return _callerList;
        }



        /// <summary>
        /// The helper function for findCalleeList
        /// </summary>
        /// <param name="m"></param>
        /// <param name="path"></param>
        /// <param name="level"></param>
        private void FindCallerListHelper(MethodDefinition m, List<MethodDefinition> path, int level) {
            if (m == null) {
                _callerList.Add(path);
                return;
            }

            path.Add(m);
            if (level > this.LEVELTHRESHOLD) {
                _callerList.Add(path);
                return;
            }


            HashSet<MethodDefinition> curCallerList = cg.ReturnCaller(m);
            if (curCallerList.Count == 0) {
                _callerList.Add(path);
                return;
            }
            foreach (MethodDefinition callerM in curCallerList) {
                FindCallerListHelper(callerM, new List<MethodDefinition>(path), level + 1);
            }
        }



        /// <summary>
        /// return all callee paths from m to all its reachable paths by function name. 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public List<List<MethodDefinition>> FindCallerListByName(String m) {
            MethodDefinition currentM = GetMethodByFullName(m);
            return FindCallerList(currentM);
        }

    }
}
