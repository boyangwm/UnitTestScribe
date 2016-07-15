﻿using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class ModelBindingTestsController : Controller
    {
        public ActionResult Index()
        {
            return View("Index", new ComplexType());
        }

        [HttpPost]
        public ActionResult SimplePost(string firstname, string lastname)
        {
            return View("Index", new ComplexType());
        }

        [HttpPost]
        public ActionResult ComplexPost(ComplexType complexType, string additionalSimpleValue)
        {
            return View("Index", new ComplexType());
        }

        public class ComplexType
        {
            public ComplexType()
            {
                this.Name = "Foo";
                this.Another = new SecondLevelComplexType();
            }

            public string Name { get; set; }
            public SecondLevelComplexType Another { get; set; }
        }

        public class SecondLevelComplexType
        {
            public SecondLevelComplexType()
            {
                this.Name = "Bar";
                this.Values = new ThirdLevelComplexType(this.Name);
            }

            public string Name { get; set; }
            public ThirdLevelComplexType Values { get; set; }
        }

        public class ThirdLevelComplexType
        {
            public ThirdLevelComplexType(string valueFor)
            {
                this.Value1 = "Value 1 for " + valueFor;
                this.Value2 = "Value 2 for " + valueFor;
            }

            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }
    }
}
