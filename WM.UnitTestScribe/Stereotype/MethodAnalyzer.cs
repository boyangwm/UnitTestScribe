using System.Collections.Generic;
using System.Linq;
using ABB.SrcML.Data;

namespace WM.UnitTestScribe.Stereotype {

    public abstract class MethodAnalyzer {

        /// <summary>
        /// The subject method
        /// </summary>
        public MethodDefinition Method { get; private set; }

        /// <summary>
        /// Checks if the method is a constructor
        /// </summary>
        public bool IsConstructor { get; private set; }

        /// <summary>
        /// Checks if the method is a IsDestructor
        /// </summary>
        public bool IsDestructor { get; private set; }


        /// <summary>
        /// Checks if the method has statements
        /// </summary>
        public bool HasStatements { get; private set; }

        /// <summary>
        /// The declaring class
        /// </summary>
        public TypeDefinition DeclaringClass { get; private set; }

        /// <summary>
        /// Checks if returns new object in the return statement
        /// </summary>
        public bool IsReturnNewObj { get; private set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public IEnumerable<VariableDeclaration> Paras { get; private set; }

        /// <summary>
        /// The return type
        /// </summary>
        public TypeUse ReturnType { get; private set; }

        /// <summary>
        /// Method parameters' information
        /// </summary>
        public HashSet<VariableInfo> ParametersInfo { get; private set; }

        /// <summary>
        /// Local variables' information
        /// </summary>
        public HashSet<VariableInfo> VariablesInfo { get; private set; }

        /// <summary>
        /// The class fields that have been assigned in the method. 
        /// </summary>
        public HashSet<VariableDeclaration> SetSelfFields { get; private set; }

        /// <summary>
        /// The class fields that have been returned in the method. 
        /// OR its info is been returned, such as "return s.GetId()" where s is the class field. 
        /// </summary>
        public HashSet<VariableDeclaration> GetSelfFields { get; private set; }


        /// <summary>
        /// The class fields that have info been returned indirectly (by local variables). 
        /// ex. "return l" where l is a local variable and l is been assigned by the class field, 
        /// </summary>
        public HashSet<VariableDeclaration> PropertyFields { get; private set; }

        /// <summary>
        /// The local methods (include inherit classes) that are invoked. 
        /// </summary>
        public HashSet<MethodDefinition> InvokedLocalMethods { get; private set; }

        /// <summary>
        /// The invoked method other than local methods (not define/use locally)
        /// </summary>
        public HashSet<MethodDefinition> InvokedExternalMethods { get; private set; }


        /// <summary>
        /// TODO: tracks no static fields info, such as public static int s = 1;
        /// </summary>
        public bool UsesNonStaticFinalField = false;


        static readonly string[] NameInclusionOperators = { ".", "->", "::" };


        private int InReturnStmt;


        /// <summary>
        /// Init the method analyzer and analyze the method automatically
        /// </summary>
        /// <param name="method"></param>
        public MethodAnalyzer(MethodDefinition method) {
            Method = method;
            ParametersInfo = new HashSet<VariableInfo>();
            VariablesInfo = new HashSet<VariableInfo>();
            SetSelfFields = new HashSet<VariableDeclaration>();
            GetSelfFields = new HashSet<VariableDeclaration>();
            PropertyFields = new HashSet<VariableDeclaration>();
            InvokedExternalMethods = new HashSet<MethodDefinition>();
            InvokedLocalMethods = new HashSet<MethodDefinition>();
            IsReturnNewObj = false;
            Analyze();
        }


        /// <summary>
        /// Analyze the method
        /// </summary>
        private void Analyze() {
            //Update basic information
            IsConstructor = Method.IsConstructor;
            IsDestructor = Method.IsDestructor;
            Paras = Method.Parameters;
            ReturnType = Method.ReturnType;

            //Method.
            DeclaringClass = Method.GetAncestors<TypeDefinition>().FirstOrDefault();

            //Initilize parameters' info
            foreach(var para in Paras) {
                VariableInfo vi = new VariableInfo(para);
                ParametersInfo.Add(vi);
            }

            var statements = Method.GetDescendantsAndSelf();
            if(statements.Count() > 1) {
                HasStatements = true;
            } else {
                HasStatements = false;
            }

            //Analyze all statement in the method
            foreach(var st in statements) {
                AnalyzeStmt(st);
            }
        }




        /// <summary>
        /// Visits and updates the method info based on the current statement
        /// </summary>
        /// <param name="st"></param>
        private void AnalyzeStmt(Statement st)
        {
            if(st is BlockStatement) {
                AnalyzeBlockStmt((BlockStatement)st);
            }
            if(st is DeclarationStatement) {
                AnalyzeDeclarationStatement((DeclarationStatement)st);
            } else if(st is ReturnStatement) {
                InReturnStmt++;
                AnalyzeReturnStmt((ReturnStatement)st);
                InReturnStmt--;
            } else {
                if(!st.GetType().IsSubclassOf(typeof(Statement))) {
                    AnalyzeOtherStmt(st);
                }
            }
        }

        //if(st is AliasStatement) {
        //        Console.WriteLine("is AliasStatement");
        //        return;
        //    }  else if(st is BreakStatement) {
        //        Console.WriteLine("is BreakStatement");
        //        return;
        //    } else if(st is ContinueStatement) {
        //        Console.WriteLine("is ContinueStatement");
        //        return;
        //    } else if(st is ExternStatement) {
        //        Console.WriteLine("is ExternStatement");
        //        return;
        //    } else if(st is GotoStatement) {
        //        Console.WriteLine("is GotoStatement");
        //        return;
        //    } else if(st is ImportStatement) {
        //        Console.WriteLine("is ImportStatement");
        //        return;
        //    } else if(st is LabelStatement) {
        //        Console.WriteLine("is LabelStatement");
        //        return;
        //    } else if(st is ThrowStatement) {
        //        Console.WriteLine("is VariableDeclaration");
        //        return;
        //    } else {
        //        if(!st.GetType().IsSubclassOf(typeof(Statement))) {
        //            VisitOtherStmt(st);
        //        }
        //        return;
        //    }


        /// <summary>
        /// Stores the local defined variable's info and updates if it has been initialized
        /// </summary>
        /// <param name="dst"></param>
        private void AnalyzeDeclarationStatement(DeclarationStatement dst) {
            //from child in dst.GetDescendantsAndSelf()
            var allDeclarations = from expression in dst.FindExpressions<VariableDeclaration>(true)
                                  select expression;

            foreach(var vd in allDeclarations) {
                UpdateByExpression(vd);
            }
        }


        /// <summary>
        /// Analyze Block statement 
        /// </summary>
        /// <param name="bs"></param>
        private void AnalyzeBlockStmt(BlockStatement bs) {
            if(bs is CatchStatement) {
            } else if(bs is ConditionBlockStatement) {
                ConditionBlockStatement cbs = (ConditionBlockStatement)bs;
                var exps = cbs.GetExpressions();
                foreach(var st in exps) {
                    UpdateByExpression(st);
                }
            } else if(bs is LockStatement) {
            } else if(bs is TryStatement) {
            } else if(bs is NamespaceDefinition) {
            } else if(bs is UsingBlockStatement) {
            }
        }


        /// <summary>
        /// Analyze return statement 
        /// </summary>
        /// <param name="rst"></param>
        private void AnalyzeReturnStmt(ReturnStatement rst) {
            //recursively analyze
            var exp = rst.Content;
            if(exp == null) {    //case of return;
                return;
            }
            UpdateByExpression(exp);


            //update name use info. 
            //simple case. Just return a NameUse. such as return c. 
            if(exp is NameUse && !exp.GetType().IsSubclassOf(typeof(NameUse))) {
                UpdateReturnInfoByNU((NameUse)exp);
            }

            //More complicated cases. 
            var subExps = exp.Components.ToList();
            int pos = 0;
            while(pos < subExps.Count()) {
                NameUse nu = FindAndUpdateNextVarUse(subExps, pos, out pos, false);
                if(nu != null) {
                    UpdateReturnInfoByNU(nu);
                }
            }
        }


        /// <summary>
        /// Analyze other kinds of statements such as assginment
        /// Only assignment, call, increment, decrement, await, and new object expressions can be use as a statment
        /// </summary>
        /// <param name="st"></param>
        private void AnalyzeOtherStmt(Statement st) {
            var exp = st.Content;
            if(exp != null) {
                UpdateByExpression(exp);
            }
        }




        /// <summary>
        /// update info based on the expression
        /// </summary>
        /// <param name="exp"></param>
        public void UpdateByExpression(Expression exp) {
            if(exp is VariableDeclaration) {
                //variable declaration expression  
                var vdExp = (VariableDeclaration)exp;
                VariableInfo vi = new VariableInfo(vdExp);
                VariablesInfo.Add(vi);

                //update if it contains assignment
                if(vdExp.Initializer != null) {
                    var subExps = new List<Expression>();
                    if(vdExp.Initializer.Components.Count == 0) {
                        subExps.Add(vdExp.Initializer);
                    } else {
                        subExps = vdExp.Initializer.Components.ToList();
                    }
                    UpdateAssignmentExpRightHand(subExps, 0, vi, false);
                    //TO DO: ex. = new school()
                }
            } else if(exp is MethodCall) {
                //Method call expression         
                var mcExp = (MethodCall)exp;
                UpdateMethodCall(mcExp);


            } else {
                    //Update the expression                         
                    var subExps = exp.Components.ToList();

                    int curPos = 0;

                    // Checks keyword new 
                    if(subExps.Count > 0) {
                        var oUse = subExps.ElementAt(0) as OperatorUse;
                        if(oUse != null && oUse.Text == "new") {
                            if(InReturnStmt > 0) {
                                IsReturnNewObj = true;
                            }
                            curPos++;
                        }
                    }
                    UpdateByExpToTheEnd(subExps, curPos);              
            }
        }



        /// <summary>
        /// iterate subExps from startPos
        /// and update if it contain any assignment operators (++, ==, =, +=, -=) 
        /// </summary>
        /// <param name="subExps"></param>
        /// <param name="startPos"></param>
        public void UpdateByExpToTheEnd(List<Expression> subExps, int startPos) {
         
            for(int curPos = startPos; curPos <= subExps.Count() - 1; curPos++) {

                Expression curExp = subExps.ElementAt(curPos);

                //Method call expression   
                if(curExp is MethodCall) {
                    UpdateMethodCall((MethodCall)curExp);
                }

                //check operator in the expression and update the information
                var opExp = curExp as OperatorUse;
                if(opExp != null) {
                    //(=, +=, -=)   Assignment category (Complicated - it can contain nested assignment)
                    if(opExp.Text == "=" || opExp.Text == "+=" || opExp.Text == "-=") {
                        //Left hand of the assignment should ONLY contains ONE variable
                        //simple NameUse
                        if(subExps.ElementAt(curPos - 1) is NameUse && !subExps.ElementAt(curPos - 1).GetType().IsSubclassOf(typeof(NameUse))) {
                            //NameUse
                            var assignedVar = (NameUse)subExps.ElementAt(curPos - 1);
                            //update if assigedVar is a field
                            VariableDeclaration assignedSelfField = FindFieldVarDecl(assignedVar);
                            if(assignedSelfField != null) {
                                SetSelfFields.Add(assignedSelfField);
                            }
                            //Check if has local variable info to update (Either parameters or local variables)
                            var varInfo = FindLocalVarInfo(assignedVar);
                            if(varInfo == null) {
                                varInfo = FindParaVarInfo(assignedVar);
                            }
                            curPos = UpdateAssignmentExpRightHand(subExps, curPos + 1, varInfo, false);
                        } else if(!subExps.ElementAt(curPos - 1).GetType().IsSubclassOf(typeof(Expression))) {
                            //If it's an Object field  (Expression)
                            bool IsFieldUpdate = false;
                            var assignedVar = GetNameUseOrObj(subExps.ElementAt(curPos - 1), out IsFieldUpdate);
                            //update if assigedVar is a field
                            VariableDeclaration assignedSelfField = FindFieldVarDecl(assignedVar);
                            if(assignedSelfField != null) {
                                SetSelfFields.Add(assignedSelfField);
                            }
                            //Check if has local variable info to update (Either parameters or local variables)
                            var varInfo = FindLocalVarInfo(assignedVar);
                            if(varInfo == null) {
                                varInfo = FindParaVarInfo(assignedVar);
                            }
                            //varInfo not null
                            curPos = UpdateAssignmentExpRightHand(subExps, curPos, varInfo, IsFieldUpdate);
                            if(varInfo == null && assignedSelfField == null) {
                                //TODO: using(var credential = new Credential())  cannot find credential definition. See summarizerTest5
                                //throw new System.Exception("should be a variable before assignment operation");
                            }
                        }
                    } else if(opExp.Text == "--" || opExp.Text == "++") {
                        //(++ --) A simple uniop , or right hand side of local field assignment (Otherwise, we called UpdateAssignmentExpRightHand)
                        if(curPos - 1 >= 0) {
                            FindAndUpdateNextVarUse(subExps, curPos - 1, out curPos, true);
                        } else {
                            FindAndUpdateNextVarUse(subExps, curPos, out curPos, true);
                        }
                        //FindAndUpdateNextVarUse(subExps, 0, out curPos, true);
                    }
                }
            }
        }

        /// <summary>
        /// Find and update the next Name use (varable use)
        /// newPos will return the new start position
        /// </summary>
        /// <param name="subExps"></param>
        /// <param name="startPos"></param>
        /// <param name="newPos"></param>
        /// <returns></returns>
        private NameUse FindAndUpdateNextVarUse(List<Expression> subExps, int startPos, out int newPos, bool IsUpdate) {
            bool haveSeenUniop = false; //have seen uniop which (should) bind with a variable
            for(int curPos = startPos; curPos <= subExps.Count() - 1; curPos++) {
                var curExp = subExps.ElementAt(curPos);

                //The subExp contains method call
                if(curExp is MethodCall && IsUpdate) {
                    UpdateMethodCall((MethodCall)curExp);
                    continue;
                }

                ////skip everyting on the left of += = or -=, if ISUpdate == false, since we do not care the rest expression
                var curOp = curExp as OperatorUse;
                if(IsUpdate == false && curOp != null && (curOp.Text == "=" || curOp.Text == "+=" || curOp.Text == "-=")) {
                    curPos = subExps.Count();
                    continue;
                }

                //record previous uniop
                if(curOp != null && (curOp.Text == "--" || curOp.Text == "++")) {
                    haveSeenUniop = true;
                    continue;
                }

                //class - might has static field visit such as int i = School.b;
                var varUse = curExp as NameUse;
                if(varUse != null) {
                    var td = varUse.FindMatches().FirstOrDefault() as TypeDefinition;
                    if(td != null) {
                        continue;
                    }
                }

                //Find next Variable use
                bool IsFieldUpdate = false;
                var curNameUse = GetNameUseOrObj(curExp, out IsFieldUpdate);


                // No deep search and update if IsUpdate is false
                if(curNameUse != null && IsUpdate == false) {
                    newPos = curPos + 1;
                    return curNameUse;
                }

                // Update and return;
                if(curNameUse != null) {
                    OperatorUse nextOp = null;
                    if(curPos + 1 <= subExps.Count() - 1) {
                        nextOp = subExps.ElementAt(curPos + 1) as OperatorUse;
                    }
                    if((nextOp != null && (nextOp.Text == "--" || nextOp.Text == "++")) || haveSeenUniop) {
                        //updae if assigedVar is a field (Field contains static field? Currently, we do not distinguish static or not. )
                        VariableDeclaration assignedSelfField = FindFieldVarDecl(curNameUse);
                        if(assignedSelfField != null) {
                            SetSelfFields.Add(assignedSelfField);
                        } else {
                            //update if assignedVar is a local variable
                            var lVarInfo = FindLocalVarInfo(curNameUse);
                            if(lVarInfo != null) {
                                lVarInfo.IsFieldChange = IsFieldUpdate;
                                lVarInfo.IsModified = true;
                            }
                            //update if assignedVar is a paramether
                            var pVarInfo = FindParaVarInfo(curNameUse);
                            if(pVarInfo != null) {
                                pVarInfo.IsFieldChange = IsFieldUpdate;
                                pVarInfo.IsModified = true;
                            }
                        }
                    }
                    if(nextOp != null && (nextOp.Text == "--" || nextOp.Text == "++"))
                        newPos = curPos + 2;
                    else
                        newPos = curPos + 1;
                    if(newPos < subExps.Count) {
                        var newOp = subExps.ElementAt(newPos) as OperatorUse;
                        if(newOp != null && (newOp.Text == "=" || newOp.Text == "+=" || newOp.Text == "-=")) {
                            //analyse all the rest expressions
                            UpdateByExpToTheEnd(subExps, curPos);
                            newPos = subExps.Count;
                        }
                    }
                    return curNameUse;
                }
            }
            newPos = subExps.Count();
            return null;
        }


        /// <summary>
        /// Update right hand side of the assignment expression
        /// </summary>
        /// <param name="subExps"></param>
        /// <param name="curPos"></param>
        /// <param name="vi"></param>
        /// <returns></returns>
        private int UpdateAssignmentExpRightHand(List<Expression> subExps, int curPos, VariableInfo vi, bool IsFieldUpdate) {
            //update if vi is a local variable
            if(vi != null) {
                vi.IsModified = true;
                vi.IsFieldChange = IsFieldUpdate;
                // check keyword new after equal =
                if(curPos < subExps.Count()) {
                    var oUse = subExps.ElementAt(curPos) as OperatorUse;
                    if(oUse != null && oUse.Text == "new") {
                        vi.IsInstantiated = true;
                        curPos++;
                    }
                }

                //Analyze the assignment values
                while(curPos <= subExps.Count() - 1) {
                    var varUse = FindAndUpdateNextVarUse(subExps, curPos, out curPos, true);
                    VariableDeclaration vd = FindFieldVarDecl(varUse);
                    if(vd != null) {
                        vi.AssignedFields.Add(vd);
                    }
                    var lVarInfo = FindLocalVarInfo(varUse);
                    if(lVarInfo != null) {
                        foreach(var feild in lVarInfo.AssignedFields) {
                            vi.AssignedFields.Add(feild);
                        }
                    }
                    var pVarInfo = FindParaVarInfo(varUse);
                    if(pVarInfo != null) {
                        foreach(var feild in pVarInfo.AssignedFields) {
                            vi.AssignedFields.Add(feild);
                        }
                    }
                }
            }
            return curPos;
        }

        /// <summary>
        /// Analyze the expression and return NameUse it belongs
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private NameUse GetNameUseOrObj(Expression exp, out bool IsFieldUpdate) {

            if(!exp.Components.Any()) {
                IsFieldUpdate = false;
                return exp as NameUse;
            }
            var compList = exp.Components.ToList();
            //check if it contains sub assignment expression
            if(compList.Count > 2) {
                UpdateByExpToTheEnd(compList, 0);

                IsFieldUpdate = false;
                for(int curPos = 0; curPos < compList.Count; curPos += 2) {
                    if(curPos + 1 < compList.Count) {
                        var expOp = compList.ElementAt(curPos + 1) as OperatorUse;
                        if(expOp != null) {
                            if(IsNameInclusionOp(expOp)) {
                                //Has connection op (Check one matching by assuming the inputs are syntax correct.)
                                IsFieldUpdate = true;
                            }
                        }
                    }

                    NameUse nu = compList.ElementAt(curPos) as NameUse;
                    if(nu != null && nu.Name != "this" && nu.Name != "base") {

                        return nu;
                    }
                }
                return null;

                //if(compList.First() is NameUse) {
                //    //Has connection op (Check one matching by assuming the inputs are syntax correct.)
                //    var expOp = compList.ElementAt(1) as OperatorUse;
                //    if(expOp != null) {
                //        foreach(var op in NameInclusionOperators) {
                //            if(op.Equals(expOp.Text)) {
                //                IsFieldUpdate = true;
                //                // check non static field
                //                var field = compList.Last() as NameUse;
                //                if(field != null && !field.GetType().IsSubclassOf(typeof(NameUse))) {
                //                    VariableDeclaration vd = field.FindMatches().FirstOrDefault() as VariableDeclaration;

                //                    //if(vd != 0 && vd.) {

                //                    //}
                //                }
                //            }
                //        }
                //    }
                //}
                //return compList.First() as NameUse;
            }
            IsFieldUpdate = false;
            return null;
        }


        private bool IsNameInclusionOp(OperatorUse opu) {
            foreach(var op in NameInclusionOperators) {
                if(op.Equals(opu.Text)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find field varible declaration for the given variable (return null if it's a local variable)
        /// </summary>
        /// <param name="vb"></param>
        /// <returns></returns>
        private VariableDeclaration FindFieldVarDecl(NameUse vb) {
            if(vb == null) {
                return null;
            }
            var decl = vb.FindMatches().FirstOrDefault() as VariableDeclaration;
            if(decl != null) {
                //if did not find in the current list, but method is matching
                MethodDefinition md = decl.ParentStatement.GetAncestorsAndSelf<MethodDefinition>().FirstOrDefault();
                if(md == null || !Method.Equals(md)) {
                    return decl;
                }
            }
            return null;
        }

        /// <summary>
        /// find and return local VariableInfo by the given NameUse         
        /// </summary>
        /// <param name="vb"></param>
        /// <returns></returns>
        private VariableInfo FindLocalVarInfo(NameUse vb) {
            if(vb == null) {
                return null;
            }
            var decl = vb.FindMatches().FirstOrDefault() as VariableDeclaration;
            if(decl != null) {
                foreach(var varInfo in VariablesInfo) {
                    if(varInfo.Variable.Equals(decl)) {
                        return varInfo;
                    }
                }
            }
            //Could be in parameters declarations
            return null;
        }

        /// <summary>
        /// Find and return parameters' VariableInfo by the given NameUse        
        /// </summary>
        /// <param name="vb"></param>
        /// <returns></returns>
        private VariableInfo FindParaVarInfo(NameUse vb) {
            if(vb == null) {
                return null;
            }
            var decl = vb.FindMatches().FirstOrDefault() as VariableDeclaration;
            if(decl != null) {
                foreach(var varInfo in ParametersInfo) {
                    if(varInfo.Variable.Equals(decl)) {
                        return varInfo;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nu"></param>
        private void UpdateReturnInfoByNU(NameUse nu) {
            VariableDeclaration vdNameUse = FindFieldVarDecl(nu);
            if(vdNameUse != null) {
                GetSelfFields.Add(vdNameUse);
            } else {
                //update if assignedVar is a local variable
                var lVarInfo = FindLocalVarInfo(nu);
                if(lVarInfo != null) {
                    foreach(var vd in lVarInfo.AssignedFields) {
                        PropertyFields.Add(vd);
                    }
                    lVarInfo.IsReturned = true;
                }
                //update if assignedVar is a paramether
                var pVarInfo = FindParaVarInfo(nu);
                if(pVarInfo != null) {
                    foreach(var vd in pVarInfo.AssignedFields) {
                        PropertyFields.Add(vd);
                    }
                    pVarInfo.IsReturned = true;
                }
            }
        }


        /// <summary>
        /// Checks the given method is a local method or not. 
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        private bool IsLocalMethod(MethodDefinition md) {
            TypeDefinition mdClass = md.GetAncestors<TypeDefinition>().FirstOrDefault();
            Queue<TypeDefinition> classSet = new Queue<TypeDefinition>();
            classSet.Enqueue(DeclaringClass);
            int i = 0;
            while(classSet.Any() && i < 10) {
                i++;
                TypeDefinition curClass = classSet.Dequeue();
                if (mdClass == null)
                    return false;
                if(mdClass.Equals(curClass)) {
                    return true;
                }
                if (curClass == null)
                    return false;
                var parentClasses = curClass.GetParentTypes(false);
                foreach(var c in parentClasses) {
                    classSet.Enqueue(c);
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mc"></param>
        private void UpdateMethodCall(MethodCall mc) {
            var args = mc.Arguments.ToList();
            var md = mc.FindMatches().FirstOrDefault() as MethodDefinition;
            if(md != null && IsLocalMethod(md)) {
                //local method 
                InvokedLocalMethods.Add(md);
            } else {
                //external method
                InvokedExternalMethods.Add(md);
            }
            foreach(var arg in args) {
                UpdateByExpression(arg);
            }
        }


        /// <summary>
        /// Checks the type is priliminary or not
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool IsPrimitiveType(TypeUse tu);



        /// <summary>
        /// Checks the return type is void or not
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool RTypeIsVoid();

        /// <summary>
        /// Checks the type is bool or not
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool RTypeIsBoolean();
    }
}
