using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Spring.Context.Support;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

// long names, Java heritage
namespace IoC.Framework.Tests.Adapters {
    public class SpringAdapter : IFrameworkAdapter {
        private readonly GenericApplicationContext context = new GenericApplicationContext();
        private readonly IObjectDefinitionFactory factory = new DefaultObjectDefinitionFactory();

        private bool contextRefreshed = false;

        public void RegisterSingleton<TComponent, TService>()
            where TComponent : TService
        {
            this.Register(typeof(TComponent), typeof(TService), true);
        }

        public void RegisterTransient<TComponent, TService>()
            where TComponent : TService
        {
            this.Register(typeof(TComponent), typeof(TService), false);
        }

        public void Register(Type componentType, Type serviceType) {
            this.Register(componentType, serviceType, false);
        }

        public void Register<TService>(TService instance) {
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

        public void RegisterAll(Assembly assembly) {
            throw new NotSupportedException();
        }

        private void EnsureContextRefreshed() {
            if (contextRefreshed)
                return;

            context.Refresh();
            contextRefreshed = true;
        }

        public TService Resolve<TService>() {
            this.EnsureContextRefreshed();

            // no generics, string access
            return (TService)context.GetObject(typeof(TService).AssemblyQualifiedName);
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            this.EnsureContextRefreshed();

            return (TService)context.GetObject(
                typeof(TService).AssemblyQualifiedName,
                additionalArguments.Values.ToArray()
            );
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }

        public bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
