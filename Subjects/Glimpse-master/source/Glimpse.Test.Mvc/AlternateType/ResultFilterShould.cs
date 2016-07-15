﻿using System.Linq;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateType;
using Glimpse.Test.Common;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.AlternateType
{
    public class ResultFilterShould
    {
        [Theory, AutoMock]
        public void SetProxyFactory(IProxyFactory proxyFactory)
        {
            AlternateType<IResultFilter> sut = new ResultFilter(proxyFactory);

            Assert.Equal(proxyFactory, sut.ProxyFactory);
        }

        [Theory, AutoMock]
        public void ReturnTwoMethods(IProxyFactory proxyFactory)
        {
            AlternateType<IResultFilter> sut = new ResultFilter(proxyFactory);

            Assert.Equal(2, sut.AllMethods.Count());
        }
    }
}
