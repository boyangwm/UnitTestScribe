
using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WM.UnitTestScribe.Summary;


namespace ABB.ChangeScribe.Test {


    public static class TestUtil {

        /// <summary> SrcML directory location </summary>
        public static readonly string SRCMLLOC = @"C:\Users\boyang.li@us.abb.com\Documents\SrcML";

        /// <summary>
        /// Creates and returns a tempral directory
        /// </summary>
        /// <returns></returns>
        public static string CreateTemporalDir() {
            var tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmpDir);
            return tmpDir;
        }


        /// <summary>
        /// Writes content to the given file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePath"></param>
        public static void WriteToFile(string content, string filePath) {
            using(StreamWriter sw = new StreamWriter(filePath)) {
                sw.WriteLine(content);
            }
        }

        /// <summary>
        /// Returns the first VariableInfo in the hashset which has the given name
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static VariableInfo FindFirstVIByName(HashSet<VariableInfo> hs, string name) {
            foreach(VariableInfo vi in hs) {
                if(vi.GetName().Equals(name)) {
                    return vi;
                }
            }          
            return null;
        }

        /// <summary>
        /// Returns the first VariableDeclaration in the hashset which has the given name
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static VariableDeclaration FindFirstVDByName(HashSet<VariableDeclaration> hs, string name) {
            foreach(VariableDeclaration vd in hs) {
                if(vd.Name.Equals(name)) {
                    return vd;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the first MethodDefinition in the hashset which has the given name
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MethodDefinition FindFirstMDByName(HashSet<MethodDefinition> hs, string name) {
            foreach(MethodDefinition vd in hs) {
                if(vd.Name.Equals(name)) {
                    return vd;
                }
            }
            return null;
        }


      

    }
}
