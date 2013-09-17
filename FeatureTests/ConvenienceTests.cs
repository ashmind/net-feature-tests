using System;
using System.ComponentModel;
using System.Diagnostics;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;
using Xunit.Sdk;

namespace DependencyInjection.FeatureTests {
    [DisplayName("Convenience")]
    [Description("Features that simplify development and reduce surprises.")]
    public class ConvenienceTests {
        [Feature]
        [DisplayName("Best constructor selection")]
        [Description(@"
            If multiple constructors are present, DI framework should select the 
            one that developer would have selected, having the same services.
            
            In this situation, it seems reasonable to select constructor with
            most resolvable dependencies.
        ")]
        [SpecialCase(typeof(SimpleInjectorAdapter), @"
            Simple Injector does not allow resolving types with multiple constructors out of the box, but this 
            behavior can be changed by replacing the Container.Options.ConstructorResolutionBehavior.
            For more info see: https://bit.ly/13WKdRT.
        ", Skip = true)]
        public void ReasonableConstructorSelection(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithMultipleConstructors>();

            var resolved = framework.Resolve<ServiceWithMultipleConstructors>();

            Assert.NotNull(resolved);
            Assert.Equal(
                ServiceWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [Feature]
        [DisplayName("Graceful recursion handling")]
        [Description(@"
            Recursive dependencies are non-resolvable, however the difference between
            getting a StackOverflowException and any other one is significant.

            StackOverflowException will crash the whole process, which is really undesirable,
            even if it is only an integration test environment. Debugging such issue can
            be a huge annoyance.
        ")]
        public void GracefulRecursionHandling(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnRecursion(framework);

            framework.Register<ServiceWithRecursiveDependency1>();
            framework.Register<ServiceWithRecursiveDependency2>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithRecursiveDependency1>(framework);
        }

        [Feature]
        [DisplayName("Graceful recursion handling (list dependency)")]
        [DependsOnFeature(typeof(ListTests), "Array")]
        public void GracefulRecursionHandlingForListDependency(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnListRecursion(framework);

            framework.Register<ServiceWithListConstructorDependency<IService[]>>();
            framework.Register<IService, ServiceThatCreatesRecursiveArrayDependency>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithListConstructorDependency<IService[]>>(framework);
        }

        private void AssertGivesCorrectExceptionWhenResolvingRecursive<TService>(IFrameworkAdapter framework) {
            try {
                framework.Resolve<TService>();
            }
            catch (Exception ex) {
                Debug.WriteLine(framework.GetType().Name + " throws following on recursion: " + ex);
                return;
            }

            throw new AssertException("No exception for recursion, either this use case is not supported or there is some other issue.");
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
