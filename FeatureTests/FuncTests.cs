using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    public class FuncTests {
        [ForEachFramework]
        public void FactoryWithNoParameters(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var func = framework.GetInstance<Func<ServiceWithSimpleConstructorDependency>>();

            Assert.NotNull(func);
            var result = func();
            Assert.NotNull(result);
        }

        [ForEachFramework]
        public void FactoryWithParameter(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithTwoConstructorDependencies>();
            var service2 = new IndependentService2();

            var func = framework.GetInstance<Func<IService2, ServiceWithTwoConstructorDependencies>>();
           
            Assert.NotNull(func);
            var result = func(service2);
            Assert.NotNull(result);
            Assert.Same(service2, result.Service2);
        }

        [ForEachFramework]
        public void TransientFactoryUsedBySingletonStillCreatesTransient(IFrameworkAdapter framework) {
            framework.RegisterTransient<IService, IndependentService>();
            framework.RegisterSingleton<ServiceWithFuncConstructorDependency>();

            var service = framework.GetInstance<ServiceWithFuncConstructorDependency>();
            var first = service.Factory();
            var second = service.Factory();

            Assert.NotSame(first, second);
        }
    }
}
