using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Spring.Context.Support;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

// long names, Java heritage
namespace FeatureTests.On.DependencyInjection.Adapters {
    public class SpringAdapter : AdapterBase {
        private readonly GenericApplicationContext context = new GenericApplicationContext();
        private readonly IObjectDefinitionFactory factory = new DefaultObjectDefinitionFactory();

        private bool contextRefreshed = false;

        public override Assembly Assembly {
            get { return typeof(IObjectDefinitionFactory).Assembly; }
        }

        public override string Name {
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

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            this.EnsureContextRefreshed();
            return this.context.GetObjectsOfType(serviceType).Values.Cast<object>();
        }
    }
}
