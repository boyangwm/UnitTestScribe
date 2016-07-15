﻿using System;
using System.Collections.Generic;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Policy;
using Glimpse.Core;
using Glimpse.Test.Core.TestDoubles;
using Glimpse.Test.Core.Tester;
using Moq;
using Xunit;

namespace Glimpse.Test.Core.Policy
{
    public class ContentTypePolicyShould:IDisposable
    {
        private ContentTypePolicyTester tester;
        public ContentTypePolicyTester Policy
        {
            get { return tester ?? (tester = ContentTypePolicyTester.Create()); }
            set { tester = value; }
        }

        public void Dispose()
        {
            Policy = null;
        }

        [Fact]
        public void RetainPolicyOnValidContentTypeWithCharset()
        {
            Policy.RequestMetadataMock.Setup(r => r.ResponseContentType).Returns("text/html; charset=utf-8");

            Assert.Equal(RuntimePolicy.On, Policy.Execute(Policy.ContextMock.Object));
        }

        [Fact]
        public void RetainPolicyOnValidContentTypes()
        {
            Assert.Equal(RuntimePolicy.On, Policy.Execute(Policy.ContextMock.Object));
        }

        [Fact]
        public void ReducePolicyOnInvalidContentTypes()
        {
            Policy.RequestMetadataMock.Setup(r => r.ResponseContentType).Returns("Unsupported/Content+Type");

            Assert.Equal(RuntimePolicy.Off, Policy.Execute(Policy.ContextMock.Object));
        }

        [Fact]
        public void ReducePolicyWithCustomRuntimPolicy()
        {
            Policy.RequestMetadataMock.Setup(r => r.ResponseContentType).Returns("application/json");

            Assert.Equal(RuntimePolicy.PersistResults, Policy.Execute(Policy.ContextMock.Object));
        }

        [Fact]
        public void ReducePolicyOnError()
        {
            var ex = new DummyException("I am a problem.");
            Policy.RequestMetadataMock.Setup(r => r.ResponseContentType).Throws(ex);

            Assert.Equal(RuntimePolicy.Off, Policy.Execute(Policy.ContextMock.Object));
            Policy.LoggerMock.Verify(l => l.Warn(It.IsAny<string>(), ex, It.IsAny<object[]>()), Times.Once());
        }

        [Fact]
        public void ConstructWithNonNullWhitelist()
        {
            Assert.NotNull(Policy.ContentTypeWhiteList);
        }

        [Fact]
        public void ConstructWithDefaultContentTypes()
        {
            Assert.True(Policy.ContentTypeWhiteList.Count > 0);
        }

        [Fact]
        public void ConstructWithWhitelistArgument()
        {
            var list = new List<Tuple<string, RuntimePolicy>>{new Tuple<string, RuntimePolicy>("anything", RuntimePolicy.PersistResults)};
            var policy = new ContentTypePolicy(list);

            Assert.Equal(list, policy.ContentTypeWhiteList);
        }

        [Fact]
        public void ThrowExceptionWhenConstructedWithNullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => new ContentTypePolicy(null));
        }

        [Fact]
        public void ExecuteOnEndRequest()
        {
            Assert.Equal(RuntimeEvent.EndRequest, Policy.ExecuteOn);
            
        }
    }
}