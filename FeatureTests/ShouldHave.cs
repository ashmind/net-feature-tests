using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;
using Xunit.Sdk;

namespace DependencyInjection.FeatureTests {
    public class ShouldHave {
        [ForEachFramework]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework) {
            framework.Add<ServiceWithSimplePropertyDependency>();
            var component = framework.GetInstance<ServiceWithSimplePropertyDependency>();

            Assert.Null(component.Service);
        }

        [ForEachFramework]
        public void CanCreateUnregisteredComponents(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();

            var resolved = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.NotNull(resolved);
            Assert.IsAssignableFrom<IndependentService>(resolved.Service);
        }

        [ForEachFramework]
        public void HandlesRecursionGracefully(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnRecursion(framework);

            framework.Add<ServiceWithRecursiveDependency1>();
            framework.Add<ServiceWithRecursiveDependency2>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithRecursiveDependency1>(framework);
        }

        [ForEachFramework]
        public void ResolvesArrayDependency(IFrameworkAdapter framework) {
            this.AssertResolvesArrayDependencyFor<ServiceWithArrayConstructorDependency>(framework);
        }

        [ForEachFramework]
        public void ResolvesArrayPropertyDependency(IFrameworkAdapter framework) {
            this.AssertResolvesArrayDependencyFor<ServiceWithArrayPropertyDependency>(framework);
        }

        public void AssertResolvesArrayDependencyFor<TTestComponent>(IFrameworkAdapter framework)
            where TTestComponent : IServiceWithArrayDependency
        {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<TTestComponent>();

            var resolved = framework.GetInstance<TTestComponent>();

            Assert.NotNull(resolved);
            Assert.NotNull(resolved.Services);
            Assert.Equal(1, resolved.Services.Length);
            Assert.IsAssignableFrom<IndependentService>(resolved.Services[0]);
        }

        [ForEachFramework]
        public void HandlesRecursionGracefullyForArrayDependency(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnListRecursion(framework);

            framework.Add<ServiceWithArrayConstructorDependency>();
            framework.Add<IEmptyService, ServiceThatCreatesRecursiveArrayDependency>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithArrayConstructorDependency>(framework);
        }

        [ForEachFramework]
        public void SelectsCorrectConstructor(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<ServiceWithMultipleConstructors>();

            var resolved = framework.GetInstance<ServiceWithMultipleConstructors>();

            Assert.NotNull(resolved);
            Assert.Equal(
                ServiceWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [ForEachFramework]
        public void SupportsOpenGenericTypes(IFrameworkAdapter framework) {
            framework.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = framework.GetInstance<IGenericService<int>>();

            Assert.NotNull(resolved);
        }
        
        private void AssertGivesCorrectExceptionWhenResolvingRecursive<TComponent>(IFrameworkAdapter framework) {
            try {
                framework.GetInstance<TComponent>();
            }
            catch (Exception ex) {
                Debug.WriteLine(framework.GetType().Name + " throws following on recursion: " + ex);
                return;
            }

            throw new AssertException("Recursion was magically solved without an exception.");      
        }
        
        private string GetFrameworkName(IFrameworkAdapter framework) {
            return Regex.Match(framework.GetType().Name, "(?<name>.+?)Adapter$").Groups["name"].Value;
        }

        private void AssertIsNotCrashingOnListRecursion(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnRecursion(framework);
            if (!framework.CrashesOnListRecursion)
                return;            
            
            throw new AssertException(string.Format(
                "{0} fails list recursion for now, and we have no way to retest it in each run (without process crash).",
                this.GetFrameworkName(framework)
            ));
        }

        private void AssertIsNotCrashingOnRecursion(IFrameworkAdapter framework) {
            if (!framework.CrashesOnRecursion)
                return;

            throw new AssertException(string.Format(
                "{0} fails recursion for now, and we have no way to retest it in each run (without process crash).",
                this.GetFrameworkName(framework)
            ));
        }
    }
}
