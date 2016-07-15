﻿using System.Linq;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateType;
using Glimpse.Test.Common;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.AlternateType
{
    public class ExceptionFilterShould
    {
        [Theory, AutoMock]
        public void SetProxyFactory(IProxyFactory proxyFactory)
        {
            AlternateType<IExceptionFilter> sut = new ExceptionFilter(proxyFactory);

            Assert.Equal(proxyFactory, sut.ProxyFactory);
        }

        [Theory, AutoMock]
        public void ReturnOneMethod(IProxyFactory proxyFactory)
        {
            AlternateType<IExceptionFilter> sut = new ExceptionFilter(proxyFactory);

            Assert.Equal(1, sut.AllMethods.Count());
        }
    }
}