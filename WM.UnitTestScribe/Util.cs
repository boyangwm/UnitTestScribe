using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.UnitTestScribe {
    class Util {

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
            using (StreamWriter sw = new StreamWriter(filePath)) {
                sw.WriteLine(content);
            }
        }



        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        public static string RandomString(int size) {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


    }
}
