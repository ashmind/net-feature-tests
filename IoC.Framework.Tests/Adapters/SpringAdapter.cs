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

        public override void RegisterSingleton<TComponent, TService>() {
            this.Register(typeof(TComponent), typeof(TService), true);
        }

        public override void RegisterTransient<TComponent, TService>() {
            this.Register(typeof(TComponent), typeof(TService), false);
        }

        public override void RegisterTransient(Type componentType, Type serviceType) {
            this.Register(componentType, serviceType, false);
        }

        public override void Register<TService>(TService instance) {
            this.context.ObjectFactory.RegisterSingleton(
                typeof(TService).AssemblyQualifiedName, instance
            );
        }

        private void Register(Type componentType, Type serviceType, bool singleton) {
            var builder = ObjectDefinitionBuilder.RootObjectDefinition(factory, componentType)
                                                 .SetAutowireMode(AutoWiringMode.AutoDetect)
                                                 .SetSingleton(singleton);

            context.RegisterObjectDefinition(serviceType.AssemblyQualifiedName, builder.ObjectDefinition);            
        }

        public override void RegisterAll(Assembly assembly) {
            throw new NotSupportedException();
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
