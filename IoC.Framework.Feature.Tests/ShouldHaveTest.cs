using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using MbUnit.Framework;

using IoC.Framework.Abstraction;
using IoC.Framework.Feature.Tests.Adapters;
using IoC.Framework.Test.Classes;

namespace IoC.Framework.Feature.Tests {
    public class ShouldHaveTest : FrameworkTestBase {
        [Test]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework) {
            framework.Add<TestComponentWithSimplePropertyDependency>();
            var component = framework.GetInstance<TestComponentWithSimplePropertyDependency>();

            Assert.IsNull(component.Service);
        }

        [Test]
        public void CanCreateUnregisteredComponents(IFrameworkAdapter framework) {
            framework.Add<ITestService, IndependentTestComponent>();

            var resolved = framework.CreateInstance<TestComponentWithSimpleConstructorDependency>();

            Assert.IsNotNull(resolved);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), resolved.Service);
        }

        [Test]
        public void HandlesRecursionGracefully(IFrameworkAdapter framework) {
            AssertIsNotCrashingOnRecursion(framework);

            framework.Add<RecursiveTestComponent1>();
            framework.Add<RecursiveTestComponent2>();

            AssertGivesCorrectExceptionWhenResolvingRecursive<RecursiveTestComponent1>(framework);
        }
                
        [Test]
        public void ResolvesArrayDependency(IFrameworkAdapter framework) {
            AssertResolvesArrayDependencyFor<TestComponentWithArrayDependency>(framework);
        }

        [Test]
        public void ResolvesArrayPropertyDependency(IFrameworkAdapter framework) {
            AssertResolvesArrayDependencyFor<TestComponentWithArrayPropertyDependency>(framework);
        }

        public void AssertResolvesArrayDependencyFor<TTestComponent>(IFrameworkAdapter framework)
            where TTestComponent : ITestComponentWithArrayDependency
        {
            framework.Add<ITestService, IndependentTestComponent>();
            framework.Add<TTestComponent>();

            var resolved = framework.GetInstance<TTestComponent>();

            Assert.IsNotNull(resolved);
            Assert.IsNotNull(resolved.Services, "Dependency is null after resolution.");
            Assert.AreEqual(1, resolved.Services.Length);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), resolved.Services[0]);
        }

        [Test]
        public void HandlesRecursionGracefullyForArrayDependency(IFrameworkAdapter framework) {
            AssertIsNotCrashingOnListRecursion(framework);

            framework.Add<TestComponentWithArrayDependency>();
            framework.Add<ITestService, TestComponentRecursingArrayDependency>();

            AssertGivesCorrectExceptionWhenResolvingRecursive<TestComponentWithArrayDependency>(framework);
        }

        [Test]
        public void SelectsCorrectConstructor(IFrameworkAdapter framework) {
            framework.Add<ITestService, IndependentTestComponent>();
            framework.Add<TestComponentWithMultipleConstructors>();

            var resolved = framework.GetInstance<TestComponentWithMultipleConstructors>();

            Assert.IsNotNull(resolved);
            Assert.AreEqual(
                TestComponentWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [Test]
        public void SupportsOpenGenericTypes(IFrameworkAdapter framework) {
            framework.AddTransient(typeof(IGenericTestService<>), typeof(GenericTestComponent<>));
            var resolved = framework.GetInstance<IGenericTestService<int>>();

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
            AssertIsNotCrashingOnRecursion(framework);
            if (!framework.CrashesOnListRecursion)
                return;            
            
            Assert.Fail(
                "{0} fails list recursion for now, and we have no way to retest it in each run (without process crash).",
                GetFrameworkName(framework)
            );
        }

        private void AssertIsNotCrashingOnRecursion(IFrameworkAdapter framework) {
            if (!framework.CrashesOnRecursion)
                return;

            Assert.Fail(
                "{0} fails recursion for now, and we have no way to retest it in each run (without process crash).",
                GetFrameworkName(framework)
            );
        }
    }
}
