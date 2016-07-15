using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WM.UnitTestScribe.Stereotype.Taxonomy {

    /// <summary>
    /// The role of stereotype. 
    /// It can be method stereotype, class stereotype, or commit stereotype 
    /// </summary>
    public enum StereotypeRole{
        Method = 0,
        Class,
        Commit,
    }


    /// <summary>
    /// The abstract class for all stereotypes.
    /// </summary>
    public abstract class Stereotype {

        /// <summary>
        /// Returns stereotype role for the child class. 
        /// </summary>
        /// <returns></returns>
        public abstract StereotypeRole GetStereotypeRole();


        /// <summary>
        /// Returns name of the stereotype. 
        /// </summary>
        /// <returns></returns>
        public abstract string GetStereotypeName();

    }
}
