using System;
using System.Collections.Generic;
using System.Linq;

using Spring.Context.Support;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

// long names, Java heritage
namespace IoC.Framework.Feature.Tests.Adapters {
    public class SpringAdapter : FrameworkAdapterBase {
        private readonly GenericApplicationContext context = new GenericApplicationContext();
        private readonly IObjectDefinitionFactory factory = new DefaultObjectDefinitionFactory();

        private bool contextRefreshed = false;

        public override void RegisterSingleton(Type serviceType, Type componentType, string key) {
            this.Register(key, componentType, serviceType, true);
        }

        public override void RegisterTransient(Type serviceType, Type componentType, string key) {
            this.Register(key, componentType, serviceType, false);
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            key = key ?? string.Format("{0} ({1})", serviceType, instance);
            this.context.ObjectFactory.RegisterSingleton(key, instance);
        }

        private void Register(string key, Type componentType, Type serviceType, bool singleton) {
            var builder = ObjectDefinitionBuilder.RootObjectDefinition(factory, componentType)
                                                 .SetAutowireMode(AutoWiringMode.AutoDetect)
                                                 .SetSingleton(singleton);

            key = key ?? string.Format("{0} ({1})", serviceType, componentType);
            context.RegisterObjectDefinition(key, builder.ObjectDefinition);            
        }

        private void EnsureContextRefreshed() {
            if (contextRefreshed)
                return;

            context.Refresh();
            contextRefreshed = true;
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            this.EnsureContextRefreshed();
            if (string.IsNullOrEmpty(key))
                return this.GetAllInstances(serviceType).First();
            
            return context.GetObject(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            this.EnsureContextRefreshed();
            return context.GetObjectsOfType(serviceType).Values.Cast<object>();
        }
    }
}
