using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StructureMap;
using StructureMap.Configuration.DSL;

namespace AshMind.Research.IoC.Frameworks.Tests.Adapters {
    public class StructureMapAdapter : IFrameworkAdapter {
        public StructureMapAdapter() {
            // In general, I like StructureMap API, but having to choose from
            //   ObjectFactory.ReInitialize();
            //   ObjectFactory.Reset();
            //   ObjectFactory.ResetDefaults();
            //   StructureMapConfiguration.ResetAll();
            // is a bit too much (each of these actualy does a different thing)

            StructureMapConfiguration.ResetAll();
            ObjectFactory.ReInitialize(); 
        }

        public void RegisterSingleton<TComponent, TService>()
            where TComponent : TService
        {
            StructureMapConfiguration.ForRequestedType<TService>()
                                     .TheDefaultIsConcreteType<TComponent>()
                                     .AsSingletons();
        }

        public void RegisterTransient<TComponent, TService>()
            where TComponent : TService
        {
            StructureMapConfiguration.ForRequestedType<TService>()
                                     .TheDefaultIsConcreteType<TComponent>();
        }

        public void Register(Type componentType, Type serviceType) {
            StructureMapConfiguration.ForRequestedType(serviceType)
                                     .TheDefaultIsConcreteType(componentType);
        }

        public void Register<TService>(TService instance) {
            StructureMapConfiguration.ForRequestedType<TService>()
                                     .TheDefaultIs(
                                          Registry.Object<TService>(instance)
                                     );
        }

        public TService Resolve<TService>() {
            return ObjectFactory.GetInstance<TService>();
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            // haven't figured out a way to do this
            throw new NotSupportedException();
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }

        public bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
