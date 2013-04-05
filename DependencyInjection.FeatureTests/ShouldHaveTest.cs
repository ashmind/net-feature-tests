using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using MbUnit.Framework;

namespace DependencyInjection.FeatureTests {
    public class ShouldHaveTest : FrameworkTestBase {
        [Test]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework) {
            framework.Add<ServiceWithSimplePropertyDependency>();
            var component = framework.GetInstance<ServiceWithSimplePropertyDependency>();

            Assert.IsNull(component.Service);
        }

        [Test]
        public void CanCreateUnregisteredComponents(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();

            var resolved = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.IsNotNull(resolved);
            Assert.IsInstanceOfType(typeof(IndependentService), resolved.Service);
        }

        [Test]
        public void HandlesRecursionGracefully(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnRecursion(framework);

            framework.Add<ServiceWithRecursiveDependency1>();
            framework.Add<ServiceWithRecursiveDependency2>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithRecursiveDependency1>(framework);
        }
                
        [Test]
        public void ResolvesArrayDependency(IFrameworkAdapter framework) {
            this.AssertResolvesArrayDependencyFor<ServiceWithArrayConstructorDependency>(framework);
        }

        [Test]
        public void ResolvesArrayPropertyDependency(IFrameworkAdapter framework) {
            this.AssertResolvesArrayDependencyFor<ServiceWithArrayPropertyDependency>(framework);
        }

        public void AssertResolvesArrayDependencyFor<TTestComponent>(IFrameworkAdapter framework)
            where TTestComponent : IServiceWithArrayDependency
        {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<TTestComponent>();

            var resolved = framework.GetInstance<TTestComponent>();

            Assert.IsNotNull(resolved);
            Assert.IsNotNull(resolved.Services, "Dependency is null after resolution.");
            Assert.AreEqual(1, resolved.Services.Length);
            Assert.IsInstanceOfType(typeof(IndependentService), resolved.Services[0]);
        }

        [Test]
        public void HandlesRecursionGracefullyForArrayDependency(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnListRecursion(framework);

            framework.Add<ServiceWithArrayConstructorDependency>();
            framework.Add<IEmptyService, ServiceThatCreatesRecursiveArrayDependency>();

            this.AssertGivesCorrectExceptionWhenResolvingRecursive<ServiceWithArrayConstructorDependency>(framework);
        }

        [Test]
        public void SelectsCorrectConstructor(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<ServiceWithMultipleConstructors>();

            var resolved = framework.GetInstance<ServiceWithMultipleConstructors>();

            Assert.IsNotNull(resolved);
            Assert.AreEqual(
                ServiceWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [Test]
        public void SupportsOpenGenericTypes(IFrameworkAdapter framework) {
            framework.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = framework.GetInstance<IGenericService<int>>();

            Assert.IsNotNull(resolved);
        }
        
        private void AssertGivesCorrectExceptionWhenResolvingRecursive<TComponent>(IFrameworkAdapter framework) {
            try {
                framework.GetInstance<TComponent>();
            }
            catch (Exception ex) {
                Assert.IsNotInstanceOfType(typeof(StackOverflowException), ex);
                Debug.WriteLine(
                    framework.GetType().Name + " throws following on recursion: " + ex
                );
                return;
            }

            Assert.Fail("Recursion was magically solved without an exception.");            
        }
        
        private string GetFrameworkName(IFrameworkAdapter framework) {
            return Regex.Match(framework.GetType().Name, "(?<name>.+?)Adapter$").Groups["name"].Value;
        }

        private void AssertIsNotCrashingOnListRecursion(IFrameworkAdapter framework) {
            this.AssertIsNotCrashingOnRecursion(framework);
            if (!framework.CrashesOnListRecursion)
                return;            
            
            Assert.Fail(
                "{0} fails list recursion for now, and we have no way to retest it in each run (without process crash).",
                this.GetFrameworkName(framework)
            );
        }

        private void AssertIsNotCrashingOnRecursion(IFrameworkAdapter framework) {
            if (!framework.CrashesOnRecursion)
                return;

            Assert.Fail(
                "{0} fails recursion for now, and we have no way to retest it in each run (without process crash).",
                this.GetFrameworkName(framework)
            );
        }
    }
}
