﻿using System;
using System.Web.Mvc;

namespace Glimpse.Test.Mvc.TestDoubles
{
    public class DummyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            throw new NotSupportedException();
        }
    }
}