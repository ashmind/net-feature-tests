using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MbUnit.Framework;

using AshMind.Research.IoC.Frameworks.Tests.Adapters;
using AshMind.Research.IoC.Frameworks.Tests.Classes;

namespace AshMind.Research.IoC.Frameworks.Tests {
    public class ShouldHaveTest : FrameworkTestBase {
        [Test]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework) {
            framework.Register<TestComponentWithSimplePropertyDependency>();
            var component = framework.Resolve<TestComponentWithSimplePropertyDependency>();

            Assert.IsNull(component.Service);
        }

        [Test]
        public void CanCreateUnregisteredComponents(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent, ITestService>();

            var resolved = framework.Create<TestComponentWithSimpleConstructorDependency>();

            Assert.IsNotNull(resolved);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), resolved.Service);
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
            framework.Register<IndependentTestComponent, ITestService>();
            framework.Register<TTestComponent>();

            var resolved = framework.Resolve<TTestComponent>();

            Assert.IsNotNull(resolved);
            Assert.IsNotNull(resolved.Services, "Dependency is null after resolution.");
            Assert.AreEqual(1, resolved.Services.Length);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), resolved.Services[0]);
        }

        [Test]
        public void SelectsCorrectConstructor(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent, ITestService>();
            framework.Register<TestComponentWithMultipleConstructors>();

            var resolved = framework.Resolve<TestComponentWithMultipleConstructors>();

            Assert.IsNotNull(resolved);
            Assert.AreEqual(
                TestComponentWithMultipleConstructors.ConstructorNames.MostResolvable,
                resolved.UsedConstructorName
            );
        }

        [Test]
        public void SupportsAdditionalArguments(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent, ITestService>();
            framework.Register<TestComponentWithAdditionalArgument>();

            var argument = new object();            
            var resolved = framework.Resolve<TestComponentWithAdditionalArgument>(new Dictionary<string, object> {
                { "additionalArgument", argument }
            });

            Assert.AreSame(argument, resolved.AdditionalArgument);
        }

        [Test]
        public void SupportsOpenGenericTypes(IFrameworkAdapter framework) {
            framework.Register(typeof(GenericTestComponent<>), typeof(IGenericTestService<>));
            var resolved = framework.Resolve<IGenericTestService<int>>();

            Assert.IsNotNull(resolved);
        }
        
        [Test]
        public void HandlesRecursionGracefully(IFrameworkAdapter framework) {
            AssertIsNotCrashingOnRecursion(framework);

            framework.Register<RecursiveTestComponent1>();
            framework.Register<RecursiveTestComponent2>();

            try {
                framework.Resolve<RecursiveTestComponent1>();
            }
            catch (Exception ex) {
                Assert.IsNotInstanceOfType(typeof(StackOverflowException), ex);
                Debug.WriteLine(
                    framework.GetType().Name + " throws following on recursion: " + ex.ToString()
                );
                return;
            }

            Assert.Fail("Recursion was magically solved without an exception.");
        }

        private void AssertIsNotCrashingOnRecursion(IFrameworkAdapter framework) {
            if (!framework.CrashesOnRecursion)
                return;

            var name = Regex.Match(framework.GetType().Name, "(?<name>.+?)Adapter$").Groups["name"].Value;
            Assert.Fail("{0} fails recursion for now, and we have no way to retest it in each run (without process crash).", name);
        }
    }
}
