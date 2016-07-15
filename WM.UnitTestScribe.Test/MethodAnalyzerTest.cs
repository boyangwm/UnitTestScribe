
using ABB.ChangeScribe.Test;
using ABB.SrcML;
using ABB.SrcML.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WM.UnitTestScribe.Summary;

namespace ABB.ChangeScribe.Test {

    [TestFixture]
    [Category("Build")]
    class CSharpAnalyzedMethodTest {

        private Dictionary<Language, AbstractCodeParser> codeParsers;
        private Dictionary<Language, SrcMLFileUnitSetup> fileSetup;

        [TestFixtureSetUp]
        public void ClassSetup() {
            codeParsers = new Dictionary<Language, AbstractCodeParser>() {
                {Language.CPlusPlus, new CPlusPlusCodeParser()},
                {Language.CSharp, new CSharpCodeParser()},
                {Language.Java, new JavaCodeParser()}
            };
            fileSetup = new Dictionary<Language, SrcMLFileUnitSetup>() {
                {Language.CPlusPlus, new SrcMLFileUnitSetup(Language.CPlusPlus)},
                {Language.CSharp, new SrcMLFileUnitSetup(Language.CSharp)},
                {Language.Java, new SrcMLFileUnitSetup(Language.Java)}
            };
        }


        [TestCase(Language.CSharp)]
        public void VAssignmentManagerTest1(Language lang) {
            //class Test {
            //    int d = 9;
            //    public void foo() {
            //        int c = 2;
            //        int b;
            //        b = c + 3;
            //    }
            //}
            string xml = @"<class pos:line=""1"" pos:column=""13"">class <name pos:line=""1"" pos:column=""19"">Test</name> <block pos:line=""1"" pos:column=""24"">{
                <decl_stmt><decl><type><name pos:line=""2"" pos:column=""17"">int</name></type> <name pos:line=""2"" pos:column=""21"">d</name> <init pos:line=""2"" pos:column=""23"">= <expr><lit:literal type=""number"" pos:line=""2"" pos:column=""25"">9</lit:literal></expr></init></decl>;</decl_stmt>
                <function><type><specifier pos:line=""3"" pos:column=""17"">public</specifier> <name pos:line=""3"" pos:column=""24"">void</name></type> <name pos:line=""3"" pos:column=""29"">foo</name><parameter_list pos:line=""3"" pos:column=""32"">()</parameter_list> <block pos:line=""3"" pos:column=""35"">{
                    <decl_stmt><decl><type><name pos:line=""4"" pos:column=""21"">int</name></type> <name pos:line=""4"" pos:column=""25"">c</name> <init pos:line=""4"" pos:column=""27"">= <expr><lit:literal type=""number"" pos:line=""4"" pos:column=""29"">2</lit:literal></expr></init></decl>;</decl_stmt>
                    <decl_stmt><decl><type><name pos:line=""5"" pos:column=""21"">int</name></type> <name pos:line=""5"" pos:column=""25"">b</name></decl>;</decl_stmt>
                    <expr_stmt><expr><name pos:line=""6"" pos:column=""21"">b</name> <op:operator pos:line=""6"" pos:column=""23"">=</op:operator> <name pos:line=""6"" pos:column=""25"">c</name> <op:operator pos:line=""6"" pos:column=""27"">+</op:operator> <lit:literal type=""number"" pos:line=""6"" pos:column=""29"">3</lit:literal></expr>;</expr_stmt>
                }</block></function>
            }</block></class>";

            var xmlElement = fileSetup[lang].GetFileUnitForXmlSnippet(xml, "test.code");
            var globalScope = codeParsers[lang].ParseFileUnit(xmlElement);

            var method = globalScope.GetDescendants<MethodDefinition>().FirstOrDefault();
            TestCaseAnalyzer ma = new TestCaseAnalyzer(method);
            var dict = ma.VAssignmentManager.VarDictionary;
            Assert.AreEqual("b", dict.ElementAt(0).Key.Name);
            var listPath = dict.ElementAt(0).Value;
            Console.WriteLine(listPath.ElementAt(0).VariableStmts.ElementAt(1).Stmt);
            Assert.AreEqual(2, listPath.ElementAt(0).VariableStmts.Count);   

        }


        [TestCase(Language.CSharp)]
        public void ObjectFieldTest2(Language lang) {
            // class Test {
            //    class Student { public int d;}
            //    class School { public int vb;}
            //    Student s = new Student();
            //    public void foo2() {
            //        School c = new School();
            //        int b;
            //        int i = 9;
            //        b = s.d - i/3;
            //        s.d = i + c.vb;
            //    }
            //}
            string xml = @"<class pos:line=""1"" pos:column=""1"">class <name pos:line=""1"" pos:column=""7"">Test</name> <block pos:line=""1"" pos:column=""12"">{
        <class pos:line=""2"" pos:column=""9"">class <name pos:line=""2"" pos:column=""15"">Student</name> <block pos:line=""2"" pos:column=""23"">{ <decl_stmt><decl><type><specifier pos:line=""2"" pos:column=""25"">public</specifier> <name pos:line=""2"" pos:column=""32"">int</name></type> <name pos:line=""2"" pos:column=""36"">d</name></decl>;</decl_stmt>}</block></class>
        <class pos:line=""3"" pos:column=""9"">class <name pos:line=""3"" pos:column=""15"">School</name> <block pos:line=""3"" pos:column=""22"">{ <decl_stmt><decl><type><specifier pos:line=""3"" pos:column=""24"">public</specifier> <name pos:line=""3"" pos:column=""31"">int</name></type> <name pos:line=""3"" pos:column=""35"">vb</name></decl>;</decl_stmt>}</block></class>
         <decl_stmt><decl><type><name pos:line=""4"" pos:column=""10"">Student</name></type> <name pos:line=""4"" pos:column=""18"">s</name> <init pos:line=""4"" pos:column=""20"">= <expr><op:operator pos:line=""4"" pos:column=""22"">new</op:operator> <call><name pos:line=""4"" pos:column=""26"">Student</name><argument_list pos:line=""4"" pos:column=""33"">()</argument_list></call></expr></init></decl>;</decl_stmt>
        <function><type><specifier pos:line=""5"" pos:column=""9"">public</specifier> <name pos:line=""5"" pos:column=""16"">void</name></type> <name pos:line=""5"" pos:column=""21"">foo2</name><parameter_list pos:line=""5"" pos:column=""25"">()</parameter_list> <block pos:line=""5"" pos:column=""28"">{
            <decl_stmt><decl><type><name pos:line=""6"" pos:column=""13"">School</name></type> <name pos:line=""6"" pos:column=""20"">c</name> <init pos:line=""6"" pos:column=""22"">= <expr><op:operator pos:line=""6"" pos:column=""24"">new</op:operator> <call><name pos:line=""6"" pos:column=""28"">School</name><argument_list pos:line=""6"" pos:column=""34"">()</argument_list></call></expr></init></decl>;</decl_stmt>
            <decl_stmt><decl><type><name pos:line=""7"" pos:column=""13"">int</name></type> <name pos:line=""7"" pos:column=""17"">b</name></decl>;</decl_stmt>
            <decl_stmt><decl><type><name pos:line=""8"" pos:column=""13"">int</name></type> <name pos:line=""8"" pos:column=""17"">i</name> <init pos:line=""8"" pos:column=""19"">= <expr><lit:literal type=""number"" pos:line=""8"" pos:column=""21"">9</lit:literal></expr></init></decl>;</decl_stmt>
            <expr_stmt><expr><name pos:line=""9"" pos:column=""13"">b</name> <op:operator pos:line=""9"" pos:column=""15"">=</op:operator> <name><name pos:line=""9"" pos:column=""17"">s</name><op:operator pos:line=""9"" pos:column=""18"">.</op:operator><name pos:line=""9"" pos:column=""19"">d</name></name> <op:operator pos:line=""9"" pos:column=""21"">-</op:operator> <name pos:line=""9"" pos:column=""23"">i</name> <op:operator pos:line=""9"" pos:column=""25"">/</op:operator> <lit:literal type=""number"" pos:line=""9"" pos:column=""27"">3</lit:literal></expr>;</expr_stmt>
            <expr_stmt><expr><name><name pos:line=""10"" pos:column=""13"">s</name><op:operator pos:line=""10"" pos:column=""14"">.</op:operator><name pos:line=""10"" pos:column=""15"">d</name></name> <op:operator pos:line=""10"" pos:column=""17"">=</op:operator> <name pos:line=""10"" pos:column=""19"">i</name> <op:operator pos:line=""10"" pos:column=""21"">+</op:operator> <name><name pos:line=""10"" pos:column=""23"">c</name><op:operator pos:line=""10"" pos:column=""24"">.</op:operator><name pos:line=""10"" pos:column=""25"">vb</name></name></expr>;</expr_stmt>
        }</block></function>
    }</block></class>";
            var xmlElement = fileSetup[lang].GetFileUnitForXmlSnippet(xml, "test.code");
            var globalScope = codeParsers[lang].ParseFileUnit(xmlElement);

            var method = globalScope.GetDescendants<MethodDefinition>().FirstOrDefault();
            TestCaseAnalyzer ma = new TestCaseAnalyzer(method);

           
        }


        [TestCase(Language.CSharp)]
        public void ObjectFieldTest3(Language lang) {
            // class Test {
            //    public void foo2() {
            //        int s = 2;
            //        int c = 5;
            //        int b;
            //        int i = 9;
            //        b = c - i/3;
            //        c = i + s;
            //    }
            //}
            string xml = @"<class pos:line=""1"" pos:column=""14"">class <name pos:line=""1"" pos:column=""20"">Test</name> <block pos:line=""1"" pos:column=""25"">{
                <function><type><specifier pos:line=""2"" pos:column=""17"">public</specifier> <name pos:line=""2"" pos:column=""24"">void</name></type> <name pos:line=""2"" pos:column=""29"">foo2</name><parameter_list pos:line=""2"" pos:column=""33"">()</parameter_list> <block pos:line=""2"" pos:column=""36"">{
                    <decl_stmt><decl><type><name pos:line=""3"" pos:column=""21"">int</name></type> <name pos:line=""3"" pos:column=""25"">s</name> <init pos:line=""3"" pos:column=""27"">= <expr><lit:literal type=""number"" pos:line=""3"" pos:column=""29"">2</lit:literal></expr></init></decl>;</decl_stmt>
                    <decl_stmt><decl><type><name pos:line=""4"" pos:column=""21"">int</name></type> <name pos:line=""4"" pos:column=""25"">c</name> <init pos:line=""4"" pos:column=""27"">= <expr><lit:literal type=""number"" pos:line=""4"" pos:column=""29"">5</lit:literal></expr></init></decl>;</decl_stmt>
                    <decl_stmt><decl><type><name pos:line=""5"" pos:column=""21"">int</name></type> <name pos:line=""5"" pos:column=""25"">b</name></decl>;</decl_stmt>
                    <decl_stmt><decl><type><name pos:line=""6"" pos:column=""21"">int</name></type> <name pos:line=""6"" pos:column=""25"">i</name> <init pos:line=""6"" pos:column=""27"">= <expr><lit:literal type=""number"" pos:line=""6"" pos:column=""29"">9</lit:literal></expr></init></decl>;</decl_stmt>
                    <expr_stmt><expr><name pos:line=""7"" pos:column=""21"">b</name> <op:operator pos:line=""7"" pos:column=""23"">=</op:operator> <name pos:line=""7"" pos:column=""25"">c</name> <op:operator pos:line=""7"" pos:column=""27"">-</op:operator> <name pos:line=""7"" pos:column=""29"">i</name><op:operator pos:line=""7"" pos:column=""30"">/</op:operator><lit:literal type=""number"" pos:line=""7"" pos:column=""31"">3</lit:literal></expr>;</expr_stmt>
                    <expr_stmt><expr><name pos:line=""8"" pos:column=""21"">c</name> <op:operator pos:line=""8"" pos:column=""23"">=</op:operator> <name pos:line=""8"" pos:column=""25"">i</name> <op:operator pos:line=""8"" pos:column=""27"">+</op:operator> <name pos:line=""8"" pos:column=""29"">s</name></expr>;</expr_stmt>
                }</block></function>
            }</block></class>";
            var xmlElement = fileSetup[lang].GetFileUnitForXmlSnippet(xml, "test.code");
            var globalScope = codeParsers[lang].ParseFileUnit(xmlElement);

            var method = globalScope.GetDescendants<MethodDefinition>().FirstOrDefault();
            TestCaseAnalyzer ma = new TestCaseAnalyzer(method);
            var dict = ma.VAssignmentManager.VarDictionary;
            Assert.AreEqual("b", dict.ElementAt(0).Key.Name);
            var listPath = dict.ElementAt(0).Value;
            Console.WriteLine(listPath.ElementAt(0).VariableStmts.ElementAt(1).Stmt);
            Assert.AreEqual(2, listPath.ElementAt(1).VariableStmts.Count);
            Assert.AreEqual("c", dict.ElementAt(1).Key.Name);
            listPath = dict.ElementAt(1).Value;
            Console.WriteLine(listPath.ElementAt(1).VariableStmts.ElementAt(1).Stmt);
            Assert.AreEqual(2, listPath.ElementAt(1).VariableStmts.Count);


            Assert.AreEqual("i", dict.ElementAt(2).Key.Name);
            Assert.AreEqual(1, dict.ElementAt(2).Value.Count);
        }

       
    }
}




