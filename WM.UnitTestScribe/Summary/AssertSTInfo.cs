using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;
using Antlr3.ST;
using WM.UnitTestScribe.Stereotype;
using WM.UnitTestScribe.Stereotype.Taxonomy;

namespace WM.UnitTestScribe.Summary
{
    public class AssertSTInfo
    {
        public VariableDeclaration Variable { get; private set; }

        public MethodDefinition Method { get; private set; }


        public readonly List<AssignmentPath> ListPath = new List<AssignmentPath>();


        public readonly Statement AssertStatment;


        public readonly Statement focalStmt;


        public AssertSTInfo(VariableDeclaration variable, VariableAssignmentManager VAssignmentManager, Statement assertStatment, MethodDefinition method)
        {
            if (variable != null)
            {
                var listPath = VAssignmentManager.GetPathByVariable(variable);
                foreach (var path in listPath)
                {
                    ListPath.Add(path);
                }
            }
            this.Variable = variable;
            this.AssertStatment = assertStatment;
            this.Method = method;
            focalStmt = FindThisFocalMethodStmt();
        }



        /// <summary>
        /// Get description for the current assertion
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string GetAssertionDescriptions(int num)
        {

            StringTemplateGroup group = new StringTemplateGroup("myGroup", @".\Templet");
            StringTemplate st = group.GetInstanceOf("AssertionTemplate");

            st.SetAttribute("Number", num);
            st.SetAttribute("Assertion", TranslateAssert(this.AssertStatment));
            //st.SetAttribute("Assertion", AssertStatment.ToString().Replace(" ", ""));


            //TODO: JavaCodeParserTests . BasicParentTest_Java  focal method is null. Handle at template
            //if (focalStmt != null) {
            //    var firstOrDefault = focalStmt.Locations.FirstOrDefault();
            //    if (firstOrDefault != null) {
            //        st.SetAttribute("FocalMethod",
            //            focalStmt.ToString().Replace(" . ", ".") + "(@line " + firstOrDefault.StartingLineNumber + ")");
            //    } else {
            //        st.SetAttribute("FocalMethod", focalStmt.ToString().Replace(" . ", "."));
            //    }
            //}

            StringBuilder rootPathDesc = new StringBuilder();
            int PathID = 1;
            VariableDeclaration LastVar = null;
            bool first = true;
            int appendNum = 0;
            bool pathNotNull = false;
            foreach (var path in ListPath)
            {
                var curFocalVar = path.VariableStmts.LastOrDefault().Variable;
                if (curFocalVar == null)
                {
                    continue;
                }
                else
                {
                    if (LastVar != curFocalVar)
                    {
                        if (!first)
                        {
                            if (rootPathDesc.ToString() != "")
                            {
                                pathNotNull = true;
                            }
                            st.SetAttribute("pathNotNull", pathNotNull);
                            st.SetAttribute("pathInfo", rootPathDesc.ToString());
                        }
                        st.SetAttribute("focalVariable", curFocalVar.Name);
                        first = false;
                        LastVar = curFocalVar;
                        appendNum = 0;
                    }

                }

                //string methodSignature = testSummary.GetMethodSignature();
                if (IsLocalPath(path))
                {
                    if (appendNum > 0)
                    {

                        rootPathDesc.Append("<br>&nbsp&nbsp&nbsp<i>" + (appendNum + 1) + ") </i>");
                    }
                    else
                    {
                        rootPathDesc.Append("<br>&nbsp&nbsp&nbsp<i>" + (appendNum + 1) + ")</i> ");
                    }
                    rootPathDesc.Append(path.GetPathDescriptions(PathID++));
                    appendNum++;
                }
            }
            if (rootPathDesc.ToString() != "")
            {
                pathNotNull = true;
            }
            st.SetAttribute("pathNotNull", pathNotNull);
            st.SetAttribute("pathInfo", rootPathDesc.ToString());
            return st.ToString();

        }


        public static string TranslateAssert(Statement stmt)
        {
            var exp = stmt.Content;
            var call = exp.Components.ElementAt(2) as MethodCall;
            if (call != null)
            {

                var arguments = call.Arguments;

                if (call.Name == "DoesNotThrow")
                {
                    return "The unit test does not throw an exception.";
                }
                else if (call.Name == "Throws")
                {
                    if (call.TypeArguments.Count > 0)
                    {
                        return "The unit test throws an " + call.TypeArguments.ElementAt(0) + " exception.";
                    }
                    return "The unit test throws an exception.";
                }
                if (arguments.Count > 0)
                {
                    if (call.Name == "IsTrue")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is true.";
                    }
                    else if (call.Name == "IsFalse")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is false.";
                    }
                    else if (call.Name == "IsNotNull")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is not null.";
                    }
                    else if (call.Name == "IsNull")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is null.";
                    }
                    else if (call.Name == "That")
                    {
                        if (arguments.Count == 1)
                        {
                            return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is true.";
                        }
                        if (arguments.Count == 2)
                        {
                            var secondArg = arguments.ElementAt(1).ToString().ToString();

                            return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i> " + secondArg.Substring(0, secondArg.Length - 1).Replace("Is . EqualTo(", "is equal to ").Replace(" . ", ".");
                        }
                    }
                    else if (call.Name == "IsInstanceOf")
                    {
                        if (call.TypeArguments.Count != 0)
                        {
                            return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is a instance of " + call.TypeArguments.ElementAt(0) + ".";
                        }
                        else if (arguments.Count == 2)
                        {
                            return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is a instance of " + arguments.ElementAt(0).ToString().Replace(" . ", ".") + ".";
                        }
                    }
                    else if (call.Name == "IsEmpty")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is empty. ";
                    }

                    else if (call.Name == "True")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is true.";
                    }
                    else if (call.Name == "False")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is false.";
                    }
                    else if (call.Name == "NotNull")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is not null.";
                    }
                    else if (call.Name == "Null")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is null.";
                    }

                    else if (call.Name == "InstanceOf")
                    {
                        if (call.TypeArguments.Count != 0)
                        {
                            return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is a instance of " + call.TypeArguments.ElementAt(0) + ".";
                        }
                        else if (arguments.Count == 2)
                        {
                            return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is a instance of " + arguments.ElementAt(0).ToString().Replace(" . ", ".") + ".";
                        }
                    }
                    else if (call.Name == "Empty")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is empty. ";
                    }
                    else if (call.Name == "NotEmpty")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is not empty. ";
                    }
                    else if (call.Name == "IsNullOrEmpty")
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is null or empty. ";
                    }
                }

                if (call.Name == "Fail")
                {
                    if (arguments.Count == 0)
                    {
                        return "Fail at line " + call.Location.StartingLineNumber + ".";
                    }
                    else
                    {
                        return "Fail at line " + call.Location.StartingLineNumber + " and " + arguments.ElementAt(0).ToString().Replace(" . ", ".") + ".";
                    }
                }
                else if (call.Name == "Inconclusive")
                {
                    return "The assertion cannot be verified at line " + call.Location.StartingLineNumber + ".";
                }
                //} else if (arguments.Count > 1) {
                if (call.Name == "AreEqual")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is equal to " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>.";
                }
                else if (call.Name == "AreSame")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " and " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " are same.";
                }
                else if (call.Name == "AreNotSame")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " and " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " are not same.";
                }
                else if (call.Name == "AreNotEqual")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is not equal to " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>.";
                }

                //} else if (arguments.Count > 1) {
                if (call.Name == "Equal")
                {
                    if (arguments.Count < 2)
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is equal.";
                    }
                    else
                    {
                        return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is equal to " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>.";
                    }
                }
                else if (call.Name == "Same")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " and " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " are same.";
                }
                else if (call.Name == "NotSame")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " and " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " are not same.";
                }
                else if (call.Name == "NotEqual")
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is not equal to " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>.";
                }

                else if (call.Name == "Contains") //Assert.Contains("A",sut.TempDataKeys)
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " contains " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>.";
                }
                else if (call.Name == "DoesNotContain") //Assert.DoesNotContain(seedBinder,ModelBinders.Binders.Values);(line 90))
                {
                    return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " does not contain " + "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>.";
                }
                else if (call.Name == "IsAssignableFrom")
                {
                    if (call.TypeArguments.Count != 0)
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is assignable from " + call.TypeArguments.ElementAt(0) + ".";
                    }
                    else if (arguments.Count == 2)
                    {
                        return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is assignable from " + arguments.ElementAt(0).ToString().Replace(" . ", ".") + ".";
                    }
                }
                else if (call.Name == "IsType") // Assert.IsType<DefaultControllerFactory>(ControllerBuilder.Current.GetControllerFactory());(line 30)
                {
                    if (call.TypeArguments.Count != 0)
                    {
                        return "<i>" + arguments.ElementAt(0).ToString().Replace(" . ", ".") + "</i>" + " is a type of " + call.TypeArguments.ElementAt(0) + ".";
                    }
                    else if (arguments.Count == 2)
                    {
                        return "<i>" + arguments.ElementAt(1).ToString().Replace(" . ", ".") + "</i>" + " is a type of " + arguments.ElementAt(0).ToString().Replace(" . ", ".") + ".";
                    }
                }
                //}
            }

            return "";
        }

        public override
            string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Assert statment: " + AssertStatment + "\n");
            builder.Append("Slicing flow : ");
            //builder.Append("Slicing flow : ");
            foreach (var path in ListPath)
            {
                builder.Append(path.ToString() + "\n");
            }
            return builder.ToString();

        }



        public Statement FindThisFocalMethodStmt()
        {
            Dictionary<VariableDeclaration, VariableAndStmt> dict = new Dictionary<VariableDeclaration, VariableAndStmt>();

            foreach (var path in ListPath)
            {
                for (int i = path.VariableStmts.Count() - 1; i >= 0; i--)
                {
                    var vs = path.VariableStmts.ElementAt(i);
                    if (!dict.ContainsKey(vs.Variable))
                    {
                        dict.Add(vs.Variable, vs);
                    }
                }
            }

            return FindFocalMethodStmt(this.Variable, this.AssertStatment, dict);
        }


        public Statement FindFocalMethodStmt(VariableDeclaration targetVariable, Statement targetStmt, Dictionary<VariableDeclaration, VariableAndStmt> dict)
        {
            var method = AssertStatment.GetAncestorsAndSelf<MethodDefinition>().FirstOrDefault();
            if (method == null)
                return null;
            if (targetStmt == null)
                return null;
            int rowLoc = Int32.MinValue;
            var firstOrDefault = targetStmt.Locations.FirstOrDefault();
            if (firstOrDefault != null)
                rowLoc = firstOrDefault.StartingLineNumber;
            //HashSet<Statement> returnStmts = new HashSet<Statement>();

            //Console.WriteLine("method  {0}", method.GetFullName());
            var mdCalls = from statments in method.GetDescendantsAndSelf()
                          from expression in statments.GetExpressions()
                          from call in expression.GetDescendantsAndSelf<MethodCall>()
                          select call;

            for (int i = mdCalls.Count() - 1; i >= 0; i--)
            {
                var call = mdCalls.ElementAt(i);
                if (call.Location.StartingLineNumber > rowLoc)
                {
                    continue;
                }
                var mdCall = call.FindMatches().FirstOrDefault() as MethodDefinition;
                if (mdCall == null)
                {
                    //Console.WriteLine("method call {0} , {1}", call, false);
                    continue;
                }
                else
                {

                    StereotypedMethod sMethod = new StereotypedMethod(mdCall);
                    //Console.WriteLine("method call {0} , {1}", call, mdCall.Name);
                    //foreach (var mType in sMethod.ListMatchedStereotypes) {
                    //    Console.WriteLine("method type {0}", mType.GetStereotypeName());
                    //}
                    bool isMutatorOrCollaborator = false;
                    foreach (var mType in sMethod.ListMatchedStereotypes)
                    {
                        if (mType.Category == Category.Collaborational || mType.Category == Category.Mutator)
                        {
                            isMutatorOrCollaborator = true;
                        }
                    }
                    if (isMutatorOrCollaborator == false)
                    {
                        continue;
                    }
                    //below: call is mutator
                    if (call.ParentExpression == null)
                    {
                        //No variable caller
                        continue;
                    }
                    var pExpression = call.ParentExpression.Components;
                    if (pExpression.Count != 3)
                    {
                        continue;
                    }
                    else
                    {
                        var invokedVar = pExpression.ElementAt(0) as NameUse;
                        if (invokedVar != null && targetVariable != null)
                        {
                            if (invokedVar.Name == targetVariable.Name)
                            {
                                //returnStmts.Add(call.ParentStatement);
                                return call.ParentStatement;
                            }
                            foreach (KeyValuePair<VariableDeclaration, VariableAndStmt> entry in dict)
                            {
                                if (invokedVar.Name == entry.Key.Name)
                                {
                                    return call.ParentStatement;
                                }
                            }
                        }
                    }
                }
            }




            //==============


            //find last mutator or collaborator in m.
            //var stmtMutator = FindLastMutatorInMethod();

            return null;

        }



        bool IsLocalPath(AssignmentPath path)
        {
            if (path.VariableStmts.Count == 0)
            {
                return false;
            }
            var root = path.VariableStmts.ElementAt(0).Variable;


            var md = root.ParentStatement.GetAncestorsAndSelf<MethodDefinition>().FirstOrDefault();
            if (md != null && md == Method)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        //FindLastMutatorInMethod(VariableDeclaration variable, MethodDefinition method) {
        //    var mdCalls = from statments in method.GetDescendantsAndSelf()
        //                  from expression in statments.GetExpressions() where expression.GetDescendantsAndSelf<MethodCall>()
        //                  from call in expression.GetDescendantsAndSelf<MethodCall>()
        //                  select call;
        //}
    }
}
