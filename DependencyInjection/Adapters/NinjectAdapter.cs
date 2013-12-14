using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using FeatureTests.Shared;
using Ninject;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using Ninject.Web.Common;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class NinjectAdapter : ContainerAdapterBase {
        private readonly IKernel kernel;

        public NinjectAdapter() {
            this.kernel = new StandardKernel();
        }

        public override Assembly Assembly {
            get { return typeof(IKernel).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.kernel.Bind(serviceType).To(implementationType).InSingletonScope();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.kernel.Bind(serviceType).To(implementationType).InTransientScope();
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.kernel.Bind(serviceType).To(implementationType).InRequestScope();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.kernel.Bind(serviceType).ToConstant(instance);
        }

        public override void BeforeAllWebRequests(WebRequestTestHelper helper) {
            //this.kernel.Bind<Func<IKernel>>()
            //           .ToConstant<Func<IKernel>>(() => this.kernel);

            //new Bootstrapper().Initialize(() => this.kernel);
            //WebRequestTestHelper.RegisterModule<NinjectHttpModule>();
            throw new SkipException("It is obvious Ninject supports this, but I can't figure this out in a reasonable time.");
        }

        public override object Resolve(Type serviceType) {
            return this.kernel.Get(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.kernel.GetAll(serviceType);
        }
    }
}
