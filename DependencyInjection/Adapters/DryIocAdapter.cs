using System;
using System.Reflection;
using System.Collections.Generic;
using DryIoc;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters
{
    public class DryIocAdapter : ContainerAdapterBase {
        private readonly Container container = new Container(
            rules => rules.With(Constructor.WithAllResolvableArguments, propertiesAndFields: PropertiesAndFields.PublicNonPrimitive)
        );

        private Container webRequestScoped;

        public override Assembly Assembly {
            get { return typeof(Container).Assembly; }
        }

        public override string PackageId {
            get { return "DryIoc.dll"; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            container.Register(serviceType, implementationType, Reuse.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            container.Register(serviceType, implementationType);
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            container.Register(serviceType, implementationType, Reuse.InCurrentScope);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            container.RegisterInstance(serviceType, instance);
        }

        public override void AfterBeginWebRequest() {
            this.webRequestScoped = container.BeginScope();
        }

        public override void BeforeEndWebRequest() {
            this.webRequestScoped.Dispose();
        }

        public override object Resolve(Type serviceType) {
            return container.Resolve(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return container.ResolveMany<object>(serviceType);
        }
    }
}
