﻿using System;
using Glimpse.AspNet.AlternateType;
using Glimpse.Core.Extensibility;
using Glimpse.Test.Common;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.AspNet.AlternateType
{
    public class RouteProcessConstraintShould
    {
        [Fact]
        public void ReturnProperMethodToImplement()
        {
            var impl = new RouteBase.ProcessConstraint();

            Assert.Equal("ProcessConstraint", impl.MethodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ReturnWhenRuntimePolicyIsOff(IAlternateMethodContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            var impl = new RouteBase.ProcessConstraint();

            impl.NewImplementation(context);

            context.Verify(c => c.Proceed());
        }

        [Theory, AutoMock]
        public void PublishMessageWhenExecuted([Frozen] IExecutionTimer timer, IAlternateMethodContext context)
        {
            context.Setup(c => c.Arguments).Returns(new object[] { (System.Web.HttpContextBase)null, new object(), (string)null, (System.Web.Routing.RouteValueDictionary)null, System.Web.Routing.RouteDirection.IncomingRequest });
            context.Setup(c => c.ReturnValue).Returns(true);
            context.Setup(c => c.InvocationTarget).Returns(new System.Web.Routing.Route("Test", null));

            var impl = new RouteBase.ProcessConstraint();

            impl.NewImplementation(context);

            timer.Verify(t => t.Time(It.IsAny<Action>()));
            context.MessageBroker.Verify(mb => mb.Publish(It.IsAny<RouteBase.ProcessConstraint.Message>()));
        }
    }
}