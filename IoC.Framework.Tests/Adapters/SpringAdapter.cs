using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Spring.Context.Support;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

// long names, Java heritage
namespace IoC.Framework.Tests.Adapters {
    public class SpringAdapter : FrameworkAdapterBase {
        private readonly GenericApplicationContext context = new GenericApplicationContext();
        private readonly IObjectDefinitionFactory factory = new DefaultObjectDefinitionFactory();

        private bool contextRefreshed = false;

        public override void RegisterSingleton(Type serviceType, Type componentType) {
            this.Register(componentType, serviceType, true);
        }

        public override void RegisterTransient(Type serviceType, Type componentType) {
            this.Register(componentType, serviceType, false);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.context.ObjectFactory.RegisterSingleton(serviceType.AssemblyQualifiedName, instance);
        }

        private void Register(Type componentType, Type serviceType, bool singleton) {
            var builder = ObjectDefinitionBuilder.RootObjectDefinition(factory, componentType)
                                                 .SetAutowireMode(AutoWiringMode.AutoDetect)
                                                 .SetSingleton(singleton);

            context.RegisterObjectDefinition(serviceType.AssemblyQualifiedName, builder.ObjectDefinition);            
        }

        private void EnsureContextRefreshed() {
            if (contextRefreshed)
                return;

            context.Refresh();
            contextRefreshed = true;
        }

        public override bool CrashesOnRecursion {
            get { return true; }
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
