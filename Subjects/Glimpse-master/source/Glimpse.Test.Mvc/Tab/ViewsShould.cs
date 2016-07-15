﻿using System;
using System.Collections.Generic;
using System.Web;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateType;
using Glimpse.Mvc.Model;
using Glimpse.Mvc.Tab;
using Glimpse.Test.Common;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.Tab
{
    public class ViewsShould
    {
        [Theory, AutoMock]
        public void Construct(Views sut)
        {
            Assert.IsAssignableFrom<ITab>(sut);
            Assert.IsAssignableFrom<ITabSetup>(sut);
        }

        [Theory, AutoMock]
        public void ExecuteOnEndRequest(Views sut)
        {
            Assert.Equal(RuntimeEvent.EndRequest, sut.ExecuteOn);
        }

        [Theory, AutoMock]
        public void HaveHttpContextBase(Views sut)
        {
            Assert.Equal(typeof(HttpContextBase), sut.RequestContextType);
        }

        [Theory, AutoMock]
        public void HaveProperName(Views sut)
        {
            Assert.Equal("Views", sut.Name);
        }

        [Theory, AutoMock]
        public void SubscribeToViewMessageTypes(Views sut, ITabSetupContext context)
        {
            sut.Setup(context);

            context.MessageBroker.Verify(mb => mb.Subscribe(It.IsAny<Action<ViewEngine.FindViews.Message>>()));
            context.MessageBroker.Verify(mb => mb.Subscribe(It.IsAny<Action<View.Render.Message>>()));
        }

        [Theory, AutoMock]
        public void HandleNullFindViewMessageCollection(Views sut, ITabContext context)
        {
            context.TabStore.Setup(ds => ds.Get(typeof(IList<ViewEngine.FindViews.Message>).AssemblyQualifiedName)).Returns<List<ViewEngine.FindViews.Message>>(null);
            context.TabStore.Setup(ds => ds.Get(typeof(IList<View.Render.Message>).AssemblyQualifiedName)).Returns(new List<View.Render.Message>());

            Assert.DoesNotThrow(() => sut.GetData(context));
        }

        [Theory, AutoMock]
        public void HandleNullViewRenderMessageCollection(Views sut, ITabContext context)
        {
            context.TabStore.Setup(ds => ds.Get(typeof(IList<ViewEngine.FindViews.Message>).AssemblyQualifiedName)).Returns(new List<ViewEngine.FindViews.Message>());
            context.TabStore.Setup(ds => ds.Get(typeof(IList<View.Render.Message>).AssemblyQualifiedName)).Returns<List<View.Render.Message>>(null);

            Assert.DoesNotThrow(() => sut.GetData(context));
        }

        [Theory, AutoMock]
        public void ReturnResult(Views sut, ITabContext context, View.Render.Arguments renderArgs, ViewEngine.FindViews.Message findViewMessage, View.Render.Message renderMessage)
        { 
            context.TabStore.Setup(ds => ds.Contains(typeof(IList<ViewEngine.FindViews.Message>).AssemblyQualifiedName)).Returns(true);
            context.TabStore.Setup(ds => ds.Get(typeof(IList<ViewEngine.FindViews.Message>).AssemblyQualifiedName)).Returns(new List<ViewEngine.FindViews.Message> { findViewMessage });
             
            context.TabStore.Setup(ds => ds.Contains(typeof(IList<View.Render.Message>).AssemblyQualifiedName)).Returns(true);
            context.TabStore.Setup(ds => ds.Get(typeof(IList<View.Render.Message>).AssemblyQualifiedName)).Returns(new List<View.Render.Message> { renderMessage });

            var result = sut.GetData(context) as List<ViewsModel>;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}