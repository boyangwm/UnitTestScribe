using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML.Data;
using ABB.Swum;
using ABB.Swum.Nodes;
using AvsAnLib;

namespace WM.UnitTestScribe.Summary {
    public class SwumSummary {


        /// <summary>
        /// The stereotyped method under analyis.
        /// </summary>
        public MethodDefinition Method { get; private set; }



        public String ClassName { get; private set; }

        /// <summary>
        /// MethodDeclaration Node in Swum
        /// </summary>
        private readonly MethodDeclarationNode _mDeclarationNode;

        /// <summary>
        /// Rule builder in Swum
        /// </summary>
        private readonly UnigramSwumBuilder _builder;



        private string _verb = "";
        private string _firstObj = "";
        private string _secondObj = "";
        private string _objConnector = "";

        /// <summary>
        /// Check if the method has been summarized or not
        /// </summary>
        public bool IsSummarized { get; private set; }


        /// <summary>
        /// Construct the method summarizer
        /// </summary>
        /// <param name="md"></param>
        public SwumSummary(MethodDefinition md) {
            this.Method = md;
            var classBelong = md.GetAncestors<TypeDefinition>().FirstOrDefault();

            //class name 
            if (classBelong != null) {
                ClassName = classBelong.Name;
            }
            else {
                ClassName = "";
            }

            //return type
            string returnType = "void";
            if (md.ReturnType != null) {
                returnType = md.ReturnType.ToString();
            }

            //Check if md returns primitive
            bool IsPrimitive = IsPrimitiveType(md.ReturnType);


            HashSet<FormalParameterRecord> paras = new HashSet<FormalParameterRecord>();
            foreach (var para in md.Parameters) {
                var vType = para.VariableType;
                var tempPara = new FormalParameterRecord(vType.ToString(), BuiltInTypeFactory.IsBuiltIn(vType), para.Name);
                paras.Add(tempPara);
            }
            MethodContext mc = new MethodContext(returnType, IsPrimitive, ClassName, paras, false, md.IsConstructor, md.IsDestructor);
            this._mDeclarationNode = new MethodDeclarationNode(md.Name, mc);
            this._builder = new UnigramSwumBuilder();
            this.IsSummarized = false;
        }


        /// <summary>
        /// Basic Summary 
        /// </summary>
        public void BasicSummary() {
            this._builder.ApplyRules(this._mDeclarationNode);

            //Constructor
            if (this._mDeclarationNode.IsConstructor && this._mDeclarationNode.Action == null) {
                //this.Mdn.Action = new WordNode("Instantiate", PartOfSpeechTag.Verb);
                this._verb = "Instantiate";
                this._firstObj = this.Method.Name;
            } else {
                if (_mDeclarationNode.Action == null)
                {
                    this._verb = "test";
                }
                else
                {
                    this._verb = _mDeclarationNode.Action.ToPlainString();
                }
                this._firstObj = ParseSwumNodeToString(_mDeclarationNode.Theme);
                //update the secondObj if Mdn has secondary argument
                if (_mDeclarationNode.SecondaryArguments != null && _mDeclarationNode.SecondaryArguments.Count != 0) {
                    var argNode = _mDeclarationNode.SecondaryArguments.ElementAt(0);
                    this._objConnector = argNode.Preposition.ToPlainString();
                    this._secondObj = argNode.ToPlainString();
                }
            }

        }


        /// <summary>
        /// Return the discription
        /// Template :   (Extra action) + Verb + firstObj + (ObjConnector) + (secondObj)
        /// </summary>
        /// <returns></returns>
        public string Describe() {
            StringBuilder description = new StringBuilder();
            if (this._mDeclarationNode.Role == MethodRole.Checker && _mDeclarationNode.Action != null && _mDeclarationNode.Action.ToPlainString().Count() > 0 && !_mDeclarationNode.Action.ToPlainString().Contains("check")) {
                description.Append("Check if ");
                if (Method.Parameters.Any()) {
                    description.Append(GetTextFromArgTypes());
                } else {
                    description.Append(ClassName);
                }
            }

            //(Extra action) + Verb + firstObj + (ObjConnector) + (secondObj)
            if (description.Length == 0)
                description.Append(this._verb);
            else
                description.Append(" " + this._verb.ToLower());
            description.Append(" " + this._firstObj.ToLower());
            description.Append(" " + this._objConnector.ToLower());
            description.Append(" " + this._secondObj.ToLower());
            //description.Append(" (Type: " + Method.ListMatchedStereotypes[0].GetStereotypeName() + ")");
            return description.ToString();
        }



      

        /// <summary>
        /// Parse <param name="nd"></param> to a string using in the summary
        /// </summary>
        /// <param name="nd"></param>
        /// <returns></returns>
        private string ParseSwumNodeToString(Node nd) {
            if (nd is EquivalenceNode) {
                var firstOrDefault = ((EquivalenceNode)nd).EquivalentNodes.FirstOrDefault();
                if (firstOrDefault != null)
                    return firstOrDefault.ToPlainString();
            }
            //we could do more than that.
            return nd.ToPlainString();
        }


        private string GetTextFromArgTypes() {
            StringBuilder description = new StringBuilder();
            int counter = 0;
            var paras = Method.Parameters;
            if (paras.Count() == 1) {
                var typeName = paras.ElementAt(0).VariableType.ToString();
                AvsAn.Result r = AvsAn.Query(typeName);
                description.Append(r.Article + " " + typeName);

            } else {
                foreach (var arg in paras) {
                    description.Append(arg.VariableType.ToString());
                    if (counter + 2 == paras.Count()) {
                        description.Append(" and ");
                    } else {
                        description.Append(", ");
                    }
                    counter++;
                }
            }
            return description.ToString();
        }




        private bool IsPrimitiveType(TypeUse tu) {
            if (tu != null) {
                return BuiltInTypeFactory.IsBuiltIn(tu);
            } else {
                return false;
            }
        }
    }
}
