using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LightCore;
using LightCore.Fluent;
using LightCore.Lifecycle;

namespace DependencyInjection.FeatureTests.Adapters {
    // thanks a lot to Philip Laureano for this adapter
    public class LightCoreAdapter : FrameworkAdapterBase {
        private IContainerBuilder builder = new ContainerBuilder();
        private IContainer container;
        
        public override Assembly FrameworkAssembly {
            get { return typeof(IContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.builder.Register(serviceType, implementationType).ControlledBy<SingletonLifecycle>();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.builder.Register(serviceType, implementationType).ControlledBy<TransientLifecycle>();
        }
        
        public override void RegisterInstance(Type serviceType, object instance) {
            // no non-generic API?
            Expression<Func<IFluentRegistration>> expression = () => this.builder.Register((object)null);
            var methodWithObject = ((MethodCallExpression)expression.Body).Method;
            var methodWithServiceType = methodWithObject.GetGenericMethodDefinition().MakeGenericMethod(serviceType);

            methodWithServiceType.Invoke(this.builder, new[] { instance });
        }

        public override object Resolve(Type serviceType) {
            this.FreezeContainer();
            return this.container.Resolve(serviceType);
        }

        private void FreezeContainer() {
            if (this.container != null)
                return;

            this.container = this.builder.Build();
            this.builder = null; // simple way to prevent accidental reuse of adapter
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
