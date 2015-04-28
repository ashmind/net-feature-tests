using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;
using FeatureTests.Shared;
using FeatureTests.On.DependencyInjection.Adapters;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.TestTypes;

namespace FeatureTests.On.DependencyInjection {
    [DisplayOrder(5)]
    [DisplayName("Func<TService>")]
    [Description(@"
        When TService is registered in a container, many frameworks automatically provide Func<..., TService>.

        This is very useful for mixing lifetimes: for example, singleton (Service) may have a Func dependency on transient (DataContext),
        which allows it to get/dispose a new instance of transient when it is needed.
    ")]
    [SpecialCase(typeof(NinjectAdapter), "Ninject supports this through Ninject.Extensions.Factory, but attempts to install it from NuGet clash with Castle version.", Skip = true)]
    public class FuncTests {
        [Feature]
        [DisplayName("No parameters")]
        [Description("Registration of `TService` automatically provides `Func<TService>`.")]
        public void FactoryWithNoParameters(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.RegisterType<ServiceWithSimpleConstructorDependency>();

            var func = adapter.Resolve<Func<ServiceWithSimpleConstructorDependency>>();

            Assert.NotNull(func);
            var result = func();
            Assert.NotNull(result);
        }

        [Feature]
        [DisplayName("Parameter for dependency")]
        [Description(@"
            Registration of `TService` automatically provides `Func<..., TService>`, where any
            arguments to the factory define constructor parameter overrides.

            For example, registration `Service(IService1, IService2)` can provide `Func<IService2, Service>`
            or `Func<IService1, Service>` or `Func<IService1, Service2, Service>`. Remaining parameters are
            provided by the container.
        ")]
        [DependsOnFeature("FactoryWithNoParameters")]
        public void FactoryWithParameter(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.RegisterType<ServiceWithTwoConstructorDependencies>();
            var service2 = new IndependentService2();

            var func = adapter.Resolve<Func<IService2, ServiceWithTwoConstructorDependencies>>();
           
            Assert.NotNull(func);
            var result = func(service2);
            Assert.NotNull(result);
            Assert.Same(service2, result.Service2);
        }

        [Feature]
        [DisplayName("Parameter for subdependency")]
        [Description(@"
            Similar to 'Parameter for dependency', but instead of a direct dependency, factory parameter
            is mapped to all levels within current resolution request.

            For example, registration `ServiceA(ServiceB1(ServiceC), IServiceB2)` can provide `Func<ServiceC, ServiceA>`.
        ")]
        [DependsOnFeature("FactoryWithParameter")]
        public void FactoryWithParameterForSubdependency(IContainerAdapter adapter) {
            adapter.RegisterType<ServiceWithSimpleConstructorDependency>();
            adapter.RegisterType<ServiceWithDependencyOnServiceWithOtherDependency>();

            var service = new IndependentService();
            var func = adapter.Resolve<Func<IService, ServiceWithDependencyOnServiceWithOtherDependency>>();

            Assert.NotNull(func);
            var result = func(service);
            Assert.NotNull(result);
            Assert.Same(service, result.Service.Service);
        }

        [Feature]
        [DisplayName("Singleton using transient")]
        [Description(@"
            When `TService` is transient and `Func<TService>` was obtained by singleton, `Func` should return new
            instance on each call.
        ")]
        [DependsOnFeature("FactoryWithNoParameters")]
        public void TransientFactoryUsedBySingletonStillCreatesTransient(IContainerAdapter adapter) {
            adapter.RegisterTransient<IService, IndependentService>();
            adapter.RegisterSingleton<ServiceWithFuncConstructorDependency>();

            var service = adapter.Resolve<ServiceWithFuncConstructorDependency>();
            var first = service.Factory();
            var second = service.Factory();

            Assert.NotSame(first, second);
        }
    }
}
