using System;
using System.ComponentModel;
using System.Diagnostics;
using Xunit;
using Xunit.Sdk;
using FeatureTests.Shared;
using FeatureTests.On.DependencyInjection.Adapters;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.TestTypes;

namespace FeatureTests.On.DependencyInjection {
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

            For more info see: https://simpleinjector.org/xtpcr.
        ", Skip = true)]
        public void ReasonableConstructorSelection(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.RegisterType<ServiceWithMultipleConstructors>();

            var resolved = adapter.Resolve<ServiceWithMultipleConstructors>();

            Assert.NotNull(resolved);
            Assert.Equal(
                ServiceWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [Feature]
        [DisplayName("Registration at any stage")]
        [Description(@"
            Some containers follow strict Register -> Build -> Resolve flow, new registrations can only be 
            added before Build.  
            This can be inconvenient for dynamic situations such as adding plug-ins.
        ")]
        [SpecialCase(typeof(SimpleInjectorAdapter), @"
            Simple Injector does not allow doing any explicit registrations after the first service has been
            resolved. The rational behind this is described here: https://simpleinjector.org/locked. That resource also
            describes how to allow registrations to be made at a later point in time.
        ", Skip = true)]
        public void RegistrationAtAnyStage(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.Resolve<IService>();
            adapter.RegisterType<IService2, IndependentService2>();

            var resolved = adapter.Resolve<IService2>();
            Assert.NotNull(resolved);
        }

        [Feature]
        [DisplayName("Graceful recursion handling")]
        [Description(@"
            Recursive dependencies are non-resolvable, however the difference between
            getting a `StackOverflowException` and any other one is significant.

            `StackOverflowException` will crash the whole process, which is really undesirable,
            even if it is only an integration test environment. Debugging such issue can
            be a huge annoyance.
        ")]
        public void GracefulRecursionHandling(IContainerAdapter adapter) {
            this.AssertIsNotCrashingOnRecursion(adapter);

            adapter.RegisterType<ServiceWithRecursiveDependency1>();
            adapter.RegisterType<ServiceWithRecursiveDependency2>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithRecursiveDependency1>(adapter);
        }

        [Feature]
        [DisplayName("Graceful recursion handling (list dependency)")]
        [DependsOnFeature(typeof(ListTests), "Array")]
        public void GracefulRecursionHandlingForListDependency(IContainerAdapter adapter) {
            this.AssertIsNotCrashingOnListRecursion(adapter);

            adapter.RegisterType<ServiceWithListConstructorDependency<IService[]>>();
            adapter.RegisterType<IService, ServiceThatCreatesRecursiveArrayDependency>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithListConstructorDependency<IService[]>>(adapter);
        }

        private void AssertGivesCorrectExceptionWhenResolvingRecursive<TService>(IContainerAdapter adapter) {
            try {
                adapter.Resolve<TService>();
            }
            catch (Exception ex) {
                Debug.WriteLine(adapter.GetType().Name + " throws following on recursion: " + ex);
                return;
            }

            throw new AssertException("No exception for recursion, either this use case is not supported or there is some other issue.");
        }

        private void AssertIsNotCrashingOnListRecursion(IContainerAdapter adapter) {
            this.AssertIsNotCrashingOnRecursion(adapter);
            if (!adapter.CrashesOnListRecursion)
                return;

            throw new AssertException(string.Format(
                "{0} fails list recursion for now, and we have no way to retest it in each run (without process crash).",
                adapter.Name
            ));
        }

        private void AssertIsNotCrashingOnRecursion(IContainerAdapter adapter) {
            if (!adapter.CrashesOnRecursion)
                return;

            throw new AssertException(string.Format(
                "{0} fails recursion for now, and we have no way to retest it in each run (without process crash).",
                adapter.Name
            ));
        }
    }
}
