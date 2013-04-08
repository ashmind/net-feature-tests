using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Spring.Context.Support;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

// long names, Java heritage
namespace DependencyInjection.FeatureTests.Adapters {
    public class SpringAdapter : FrameworkAdapterBase {
        private readonly GenericApplicationContext context = new GenericApplicationContext();
        private readonly IObjectDefinitionFactory factory = new DefaultObjectDefinitionFactory();

        private bool contextRefreshed = false;

        public override Assembly FrameworkAssembly {
            get { return typeof(IObjectDefinitionFactory).Assembly; }
        }

        public override string FrameworkName {
            get { return "Spring.NET"; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.Register(implementationType, serviceType, true);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.Register(implementationType, serviceType, false);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            var key = string.Format("{0} ({1})", serviceType, instance);
            this.context.ObjectFactory.RegisterSingleton(key, instance);
        }

        private void Register(Type implementationType, Type serviceType, bool singleton) {
            var builder = ObjectDefinitionBuilder.RootObjectDefinition(this.factory, implementationType)
                                                 .SetAutowireMode(AutoWiringMode.AutoDetect)
                                                 .SetSingleton(singleton);

            var key = string.Format("{0} ({1})", serviceType, implementationType);
            this.context.RegisterObjectDefinition(key, builder.ObjectDefinition);            
        }

        private void EnsureContextRefreshed() {
            if (this.contextRefreshed)
                return;

            this.context.Refresh();
            this.contextRefreshed = true;
        }

        public override object Resolve(Type serviceType) {
            this.EnsureContextRefreshed();
            return this.context.GetObjectsOfType(serviceType).Values.Cast<object>().Single();
        }
    }
}
