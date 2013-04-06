using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    public class ListTests {
        [ForEachFramework]
        public void ListConstructorDependency(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithArrayConstructorDependency>(framework);
        }

        [ForEachFramework]
        public void ListPropertyDependency(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithArrayPropertyDependency>(framework);
        }

        public void AssertResolvesListDependencyFor<TTestComponent>(IFrameworkAdapter framework)
            where TTestComponent : IServiceWithArrayDependency {
            framework.Register<IService, IndependentService>();
            framework.Register<TTestComponent>();

            var resolved = framework.GetInstance<TTestComponent>();

            Assert.NotNull(resolved);
            Assert.NotNull(resolved.Services);
            Assert.Equal(1, resolved.Services.Length);
            Assert.IsAssignableFrom<IndependentService>(resolved.Services[0]);
        }
    }
}
