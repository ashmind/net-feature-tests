using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;
using Xunit.Sdk;

namespace DependencyInjection.FeatureTests {
    public class ConvenienceTests {
        [ForEachFramework]
        public void UnregisteredService(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();

            var resolved = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.NotNull(resolved);
            Assert.IsAssignableFrom<IndependentService>(resolved.Service);
        }

        [ForEachFramework]
        public void ReasonableConstructorSelection(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithMultipleConstructors>();

            var resolved = framework.GetInstance<ServiceWithMultipleConstructors>();

            Assert.NotNull(resolved);
            Assert.Equal(
                ServiceWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [ForEachFramework]
        public void GracefulRecursionHandling(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnRecursion(framework);

            framework.Register<ServiceWithRecursiveDependency1>();
            framework.Register<ServiceWithRecursiveDependency2>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithRecursiveDependency1>(framework);
        }

        [ForEachFramework]
        public void GracefulRecursionHandlingForListDependency(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnListRecursion(framework);

            framework.Register<ServiceWithArrayConstructorDependency>();
            framework.Register<IService, ServiceThatCreatesRecursiveArrayDependency>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithArrayConstructorDependency>(framework);
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

        private void AssertIsNotCrashingOnListRecursion(IFrameworkAdapter adapter) {
            this.AssertIsNotCrashingOnRecursion(adapter);
            if (!adapter.CrashesOnListRecursion)
                return;

            throw new AssertException(string.Format(
                "{0} fails list recursion for now, and we have no way to retest it in each run (without process crash).",
                adapter.FrameworkName
            ));
        }

        private void AssertIsNotCrashingOnRecursion(IFrameworkAdapter adapter) {
            if (!adapter.CrashesOnRecursion)
                return;

            throw new AssertException(string.Format(
                "{0} fails recursion for now, and we have no way to retest it in each run (without process crash).",
                adapter.FrameworkName
            ));
        }
    }
}
