﻿using System;
using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy;
using Glimpse.Core.Extensibility;
using Moq;
using Xunit;

namespace Glimpse.Test.Core.Extensibility
{
    public class AlternateTypeToCastleInterceptorAdapterShould
    {
        [Fact]
        public void Construct()
        {
            var implementationMock = new Mock<IAlternateMethod>();
            var loggerMock = new Mock<ILogger>();

            var adapter = new AlternateTypeToCastleInterceptorAdapter(implementationMock.Object, loggerMock.Object, new Mock<IMessageBroker>().Object, new Mock<IProxyFactory>().Object, () => new ExecutionTimer(Stopwatch.StartNew()), () => RuntimePolicy.On);

            Assert.Equal(implementationMock.Object, adapter.Implementation);
            Assert.Equal(loggerMock.Object, adapter.Logger);
        }

        [Fact]
        public void ThrowWithNullConstructorParameters()
        {
            var implementationMock = new Mock<IAlternateMethod>();
            var loggerMock = new Mock<ILogger>();

            Assert.Throws<ArgumentNullException>(() => new AlternateTypeToCastleInterceptorAdapter(null, loggerMock.Object, new Mock<IMessageBroker>().Object, new Mock<IProxyFactory>().Object, () => new ExecutionTimer(Stopwatch.StartNew()), () => RuntimePolicy.On));
            Assert.Throws<ArgumentNullException>(() => new AlternateTypeToCastleInterceptorAdapter(implementationMock.Object, null, new Mock<IMessageBroker>().Object, new Mock<IProxyFactory>().Object, () => new ExecutionTimer(Stopwatch.StartNew()), () => RuntimePolicy.On));
        }

        [Fact]
        public void PassThroughMethodToImplement()
        {
            var expected = GetType().GetMethods().First();
            var implementationMock = new Mock<IAlternateMethod>();
            implementationMock.Setup(i => i.MethodToImplement).Returns(expected);
            var loggerMock = new Mock<ILogger>();

            var adapter = new AlternateTypeToCastleInterceptorAdapter(implementationMock.Object, loggerMock.Object, new Mock<IMessageBroker>().Object, new Mock<IProxyFactory>().Object, () => new ExecutionTimer(Stopwatch.StartNew()), () => RuntimePolicy.On);

            Assert.Equal(expected, adapter.MethodToImplement);
        }

        [Fact]
        public void Intercept()
        {
            var implementationMock = new Mock<IAlternateMethod>();
            var loggerMock = new Mock<ILogger>();

            var adapter = new AlternateTypeToCastleInterceptorAdapter(implementationMock.Object, loggerMock.Object, new Mock<IMessageBroker>().Object, new Mock<IProxyFactory>().Object, () => new ExecutionTimer(Stopwatch.StartNew()), () => RuntimePolicy.On);

            var invocationMock = new Mock<IInvocation>();

            adapter.Intercept(invocationMock.Object);

            implementationMock.Verify(i => i.NewImplementation(It.IsAny<IAlternateMethodContext>()));
        }
    }
}