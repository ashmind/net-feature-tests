using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IfInjector;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class IfInjectorAdapter : ContainerAdapterBase {
        private readonly Injector injector = new Injector();
        
        public override Assembly Assembly {
            get { return typeof(Injector).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            GenericHelper.RewriteAndInvoke(
                () => this.injector.Register(Binding.For<P<X1>>().To<C<X2, X1>>().AsSingleton(true)),
                serviceType, implementationType
            );
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            GenericHelper.RewriteAndInvoke(
                () => this.injector.Register(Binding.For<P<X1>>().To<C<X2, X1>>()),
                serviceType, implementationType
            );
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBeProvided();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            GenericHelper.RewriteAndInvoke(
                () => this.injector.Register(Binding.For<X1>().SetFactory(() => (X1)instance).AsSingleton(true)),
                serviceType
            );
        }

        public override object Resolve(Type serviceType) {
            return this.injector.Resolve(serviceType);
        }
    }
}
