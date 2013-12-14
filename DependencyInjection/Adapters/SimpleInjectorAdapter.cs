using System;
using System.Collections.Generic;
using System.Reflection;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using SimpleInjector.Integration.Web;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class SimpleInjectorAdapter : ContainerAdapterBase {
        private readonly Container container = new Container();

        public override string Name {
            get { return "Simple Injector"; }
        }

        public override Assembly Assembly {
            get { return typeof(Container).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, Lifestyle.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, Lifestyle.Transient);
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, new WebRequestLifestyle(true));
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.RegisterSingle(serviceType, instance);
        }

        public override void BeforeAllWebRequests(WebRequestTestHelper helper) {
            helper.RegisterModule<SimpleInjectorHttpModule>();
        }

        public override object Resolve(Type serviceType) {
            return this.container.GetInstance(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.container.GetAllInstances(serviceType);
        }

        private void Register(Type serviceType, Type implementationType, Lifestyle lifestyle) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;

            if (isOpenGeneric) {
                this.container.RegisterOpenGeneric(serviceType, implementationType, lifestyle);
            }
            else {
                this.container.Register(serviceType, implementationType, lifestyle);

                // Registering collections in Simple Injector is done using the RegisterAll overloads, but
                // this forces all elements to be registered at once. For integration scenarios, the
                // AppendToCollection extension method can be used. This allows adding elements to a collection
                // one by one.
                this.container.AppendToCollection(serviceType,
                    lifestyle.CreateRegistration(serviceType, implementationType, this.container));
            }
        }
    }
}