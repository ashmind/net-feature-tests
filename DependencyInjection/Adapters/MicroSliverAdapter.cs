using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using MicroSliver;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class MicroSliverAdapter : ContainerAdapterBase {
        private readonly IoC ioc = new IoC();

        #region DelegateCreator

        private class DelegateCreator : ICreator {
            private readonly Func<object> create;

            public DelegateCreator(Func<object> create) {
                this.create = create;
            }

            public object Create() {
                return this.create();
            }
        }

        #endregion
        
        public override Assembly Assembly {
            get { return typeof(IoC).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            GenericHelper.RewriteAndInvoke(
                () => this.ioc.Map<P<X1>, C<X2, X1>>().ToSingletonScope(),
                serviceType, implementationType
            );
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            GenericHelper.RewriteAndInvoke(
                () => this.ioc.Map<P<X1>, C<X2, X1>>().ToInstanceScope(),
                serviceType, implementationType
            );
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            GenericHelper.RewriteAndInvoke(
                () => this.ioc.Map<P<X1>, C<X2, X1>>().ToRequestScope(),
                serviceType, implementationType
            );
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            GenericHelper.RewriteAndInvoke(
                () => this.ioc.Map<X1>(new DelegateCreator(() => instance)),
                serviceType
            );
        }

        public override void BeforeAllWebRequests(WebRequestTestHelper helper) {
            helper.RegisterModule<MicroSliverHttpRequestModule>();
        }
        
        public override object Resolve(Type serviceType) {
            return this.ioc.GetByType(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
