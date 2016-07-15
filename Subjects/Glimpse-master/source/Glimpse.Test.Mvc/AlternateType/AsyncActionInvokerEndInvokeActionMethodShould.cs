﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateType;
using Glimpse.Test.Common;
using Glimpse.Test.Mvc.TestDoubles;

using Moq;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.AlternateType
{
    public class AsyncActionInvokerEndInvokeActionMethodShould
    {
        [Fact]
        public void ImplementProperMethod()
        {
            var sut = new AsyncActionInvoker.EndInvokeActionMethod();

            Assert.Equal("EndInvokeActionMethod", sut.MethodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ProceedThenReturnWithRuntimePolicyOff(AsyncActionInvoker.EndInvokeActionMethod sut, IAlternateMethodContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
            context.Verify(c => c.Proxy, Times.Never());
        }

        [Theory, AutoMock]
        public void PublishMessageWithRuntimePolicyOn(AsyncActionInvoker.EndInvokeActionMethod sut, IAlternateMethodContext context, ActionDescriptor actionDescriptor)
        {
            var actionDescriptorMock = new Mock<ActionDescriptor>();
            actionDescriptorMock.Setup(a => a.ControllerDescriptor).Returns(new ReflectedControllerDescriptor(typeof(DummyController)));
            actionDescriptorMock.Setup(a => a.ActionName).Returns("Index");

            context.Setup(c => c.ReturnValue).Returns(new ContentResult());
            context.Setup(c => c.Proxy).Returns(
                new ActionInvokerStateMixin
                {
                    Offset = TimeSpan.Zero, 
                    Arguments = new ActionInvoker.InvokeActionMethod.Arguments(new ControllerContext(), actionDescriptor, new Dictionary<string, object>())
                });
            context.Setup(c => c.Arguments).Returns(new object[]
                                                            {
                                                                new ControllerContext(),
                                                                actionDescriptorMock.Object,
                                                                new Dictionary<string, object>()
                                                            });

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
            context.MessageBroker.Verify(b => b.Publish(It.IsAny<ActionInvoker.InvokeActionMethod.Message>()));
        }
    }
}