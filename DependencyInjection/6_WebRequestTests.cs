using System;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using Xunit;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.TestTypes;
using FeatureTests.Shared;

namespace FeatureTests.On.DependencyInjection
{
    [DisplayOrder(6)]
    [DisplayName("Request lifetime")]
    [Description(@"
        Web applications are a very common use for DI containers.  
        For those, a per-web-request lifetime can be very useful (for things like ORM sessions and transactions).  
        Note that it is not the same as per-thread lifetime, as the request can potentially switch threads.

        This feature is often a more limited version of child container/scope feature, but as it is
        more widely supported and more obviously useful, for now I focus only on web requests.
    ")]
    public class WebRequestTests : IDisposable {
        private readonly WebRequestTestHelper helper;

        public WebRequestTests() {
            this.helper = new WebRequestTestHelper();
        }

        [Feature]
        [DisplayName("Basic support")]
        [Description("Verifies whther per-request lifetime is supported at all.")]
        public void PerRequestSupport(IContainerAdapter adapter) {
            adapter.RegisterPerWebRequest<IndependentService>();
        }

        [Feature]
        [DisplayName("Instance is reused within request")]
        [DependsOnFeature("PerRequestSupport")]
        public void ReuseWithinRequest(IContainerAdapter adapter) {
            adapter.RegisterPerWebRequest<IndependentService>();

            BeginRequest(adapter);
            var first = adapter.Resolve<IndependentService>();
            var second = adapter.Resolve<IndependentService>();
            EndRequest(adapter);

            Assert.Same(first, second);
        }

        [Feature]
        [DisplayName("Instance is not reused between requests")]
        [DependsOnFeature("PerRequestSupport")]
        public void NoReuseBetweenRequests(IContainerAdapter adapter) {
            adapter.RegisterPerWebRequest<IndependentService>();

            BeginRequest(adapter);
            var first = adapter.Resolve<IndependentService>();
            EndRequest(adapter);

            BeginRequest(adapter);
            var second = adapter.Resolve<IndependentService>();
            EndRequest(adapter);

            Assert.NotSame(first, second);
        }

        [Feature]
        [DisplayName("Instance is disposed at the end of request")]
        [DependsOnFeature("PerRequestSupport")]
        public void ComponentIsDisposedAtTheEndOfRequest(IContainerAdapter adapter) {
            adapter.RegisterPerWebRequest<DisposableService>();

            BeginRequest(adapter);
            var service = adapter.Resolve<DisposableService>();
            EndRequest(adapter);

            Assert.True(service.Disposed);
        }

        [Feature]
        [DisplayName("Singleton using factory does not reuse instance between requests")]
        [DependsOnFeature("PerRequestSupport")]
        [DependsOnFeature(typeof(FuncTests), "FactoryWithNoParameters")]
        public void FactoryNoReuseBetweenRequests(IContainerAdapter adapter) {
            adapter.RegisterPerWebRequest<IService, IndependentService>();
            adapter.RegisterSingleton<ServiceWithFuncConstructorDependency>();

            var service = adapter.Resolve<ServiceWithFuncConstructorDependency>();

            BeginRequest(adapter);
            var first = service.Factory();
            EndRequest(adapter);

            BeginRequest(adapter);
            var second = service.Factory();
            EndRequest(adapter);

            Assert.NotSame(first, second);
        }

        private bool isFirstWebRequest = true;
        private void BeginRequest(IContainerAdapter adapter) {
            if (isFirstWebRequest) {
                adapter.BeforeAllWebRequests(helper);
                isFirstWebRequest = false;
            }

            this.helper.BeginWebRequest();
            adapter.AfterBeginWebRequest();
        }

        private void EndRequest(IContainerAdapter adapter) {
            adapter.BeforeEndWebRequest();
            this.helper.EndWebRequest();
        }
        
        public void Dispose() {
            this.helper.Dispose();
        }
    }
}
