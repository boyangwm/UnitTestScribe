﻿using System.Linq;
using Glimpse.AspNet.AlternateType;
using Glimpse.Core.Extensibility;
using Glimpse.Test.Common;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.AspNet.AlternateType
{
    public class RouteConstraintShould
    {
        [Theory, AutoMock]
        public void ReturnOneMethod(IProxyFactory proxyFactory)
        {
            AlternateType<System.Web.Routing.IRouteConstraint> alternationImplementation = new RouteConstraint(proxyFactory);

            Assert.Equal(1, alternationImplementation.AllMethods.Count());
        }

        [Theory, AutoMock]
        public void SetProxyFactory(IProxyFactory proxyFactory)
        {
            AlternateType<System.Web.Routing.IRouteConstraint> alternationImplementation = new RouteConstraint(proxyFactory);

            Assert.Equal(proxyFactory, alternationImplementation.ProxyFactory);
        }
    }
}
