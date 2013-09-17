using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IfInjector;
using IfInjector.IfCore;

namespace DependencyInjection.FeatureTests.Adapters {
    public class IfInjectorAdapter : FrameworkAdapterBase {
        #region Binder
        // needed because there does not seem to be a non-generic API

        private interface IBinder {
            void BindTransient();
            void BindSingleton();
            void BindInstance(object instance);
        }

        private class Binder<TService, TImplementation> : IBinder
            where TService : class
            where TImplementation : class, TService 
        {
            private readonly Injector injector;

            public Binder(Injector injector) {
                this.injector = injector;
            }

            public void BindTransient() {
                this.injector.Bind<TService, TImplementation>();
            }

            public void BindSingleton() {
                this.injector.Bind<TService, TImplementation>().AsSingleton();
            }

            public void BindInstance(object instance) {
                this.injector.Bind<TService>().SetFactoryLambda((Expression<Func<TService>>)(() => (TService)instance)).AsSingleton();
            }
        }
        #endregion

        private readonly Injector injector = new Injector();
        
        public override Assembly FrameworkAssembly {
            get { return typeof(Injector).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            CreateBinder(serviceType, implementationType).BindSingleton();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            CreateBinder(serviceType, implementationType).BindTransient();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            CreateBinder(serviceType, instance.GetType()).BindInstance(instance);
        }

        public override object Resolve(Type serviceType) {
            return this.injector.Resolve(serviceType);
        }

        private IBinder CreateBinder(Type serviceType, Type implementationType) {
            return (IBinder)Activator.CreateInstance(
                typeof(Binder<,>).MakeGenericType(serviceType, implementationType),
                this.injector
            );
        }
    }
}
