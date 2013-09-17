using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dynamo.Ioc;

namespace DependencyInjection.FeatureTests.Adapters {
    public class DynamoIocAdapter : FrameworkAdapterBase {
        private readonly IocContainer container = new IocContainer();

        public override string FrameworkName {
            get { return "Dynamo.Ioc"; }
        }
        
        public override Assembly FrameworkAssembly {
            get { return typeof(IocContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).SetLifetime(new ContainerLifetime());
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).SetLifetime(new TransientLifetime());
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.RegisterInstance(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
