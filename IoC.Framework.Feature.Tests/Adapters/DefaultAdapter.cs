using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Abstraction;
using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Feature.Tests.Adapters {
    public class DefaultAdapter : FrameworkAdapterBase {
        private readonly IServiceInjectionFramework framework;
        private readonly IServiceContainer container;
        private IServiceLocator locator;

        private readonly bool crashesOnRecursion;

        public DefaultAdapter(IServiceInjectionFramework framework, bool crashesOnRecursion) {
            this.framework = framework;
            this.container = framework.CreateContainer();

            this.crashesOnRecursion = crashesOnRecursion;
        }

        private void EnsureLocator() {
            if (this.locator != null)
                return;

            this.locator = this.framework.CreateLocator(this.container);
        }

        public override void AddTransient(Type serviceType, Type componentType, string key) {
            this.container.AddTransient(serviceType, componentType, key);
        }

        public override void AddSingleton(Type serviceType, Type componentType, string key) {
            this.container.AddSingleton(serviceType, componentType, key);
        }

        public override void AddInstance(Type serviceType, object instance, string key) {
            this.container.AddInstance(serviceType, instance, key);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            this.EnsureLocator();
            return this.locator.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            this.EnsureLocator();
            return this.locator.GetAllInstances(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return this.crashesOnRecursion; }
        }
    }
}
