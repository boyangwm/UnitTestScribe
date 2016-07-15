﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateType;
using Glimpse.Test.Common;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.AlternateType
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Class is okay because it only changes the generic T parameter for the abstract class below.")]
    public class ControllerActionInvokerGetFiltersShould : ActionInvokerGetFiltersShould<ControllerActionInvoker>
    {
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Class is okay because it only changes the generic T parameter for the abstract class below.")]
    public class AsyncControllerActionInvokerGetFiltersShould : ActionInvokerGetFiltersShould<AsyncControllerActionInvoker>
    {
    }

    public abstract class ActionInvokerGetFiltersShould<T> where T : class
    {
        [Theory, AutoMock]
        public void ImplementProperMethod(ActionInvoker.GetFilters<T> sut)
        {
            Assert.Equal("GetFilters", sut.MethodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ProceedAndReturnWithRuntimePolicyOff(ActionInvoker.GetFilters<T> sut, IAlternateMethodContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
            context.Verify(c => c.ReturnValue, Times.Never());
        }

        [Theory, AutoMock]
        public void ProxyFiltersWithRuntimePolicyOn([Frozen] IExecutionTimer timer, ActionInvoker.GetFilters<T> sut, IAlternateMethodContext context)
        {
            sut.NewImplementation(context);

            timer.Verify(t => t.Time(It.IsAny<Action>()));
            context.Verify(c => c.ReturnValue);
        }
    }
}