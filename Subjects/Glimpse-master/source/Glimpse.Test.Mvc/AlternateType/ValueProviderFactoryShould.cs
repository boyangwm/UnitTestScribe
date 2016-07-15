﻿using System.Linq;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Test.Common;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.AlternateType
{
    public class ValueProviderFactoryShould
    {
        [Theory, AutoMock]
        public void Construct(IProxyFactory proxyFactory)
        {
            var sut = new Glimpse.Mvc.AlternateType.ValueProviderFactory(proxyFactory);

            Assert.NotNull(sut);
            Assert.IsAssignableFrom<AlternateType<ValueProviderFactory>>(sut);
        }

        [Theory, AutoMock]
        public void ImplementOneMethod(Glimpse.Mvc.AlternateType.ValueProviderFactory sut)
        {
            Assert.Equal(1, sut.AllMethods.Count());
        }
    }
}