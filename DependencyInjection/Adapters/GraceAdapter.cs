using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using Grace.DependencyInjection;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using Grace.MVC.DependencyInjection;
using Grace.MVC.Extensions;

namespace FeatureTests.On.DependencyInjection.Adapters {
	public class GraceAdapter : ContainerAdapterBase {
		private readonly DependencyInjectionContainer container;

		public GraceAdapter() {
			this.container = new DependencyInjectionContainer ();
			this.container.Configure(c => c.Export<WebSharedPerRequestLifestyleProvider>().ByInterfaces());
		}

	    public override Assembly Assembly {
			get { return typeof(DependencyInjectionContainer).Assembly; }
		}

		public override void RegisterTransient(Type serviceType, Type implementationType) {
			this.container.Configure(c => c.Export(implementationType).As(serviceType).AutoWireProperties());
		}

		public override void RegisterSingleton(Type serviceType, Type implementationType) {
			this.container.Configure(c => c.Export(implementationType).As(serviceType).Lifestyle.Singleton());
		}

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.container.Configure(c => c.Export(implementationType).As(serviceType).Lifestyle.SingletonPerRequest());
        }

		public override void RegisterInstance(Type serviceType, object instance) {
			this.container.Configure(c => c.ExportInstance(instance).As(serviceType));
		}

		public override void BeforeAllWebRequests(WebRequestTestHelper helper) {
			helper.RegisterModule(new DisposableHttpModule());
        }

		public override object Resolve(Type serviceType) {
			return this.container.Locate(serviceType);
		}

		public override IEnumerable<object> ResolveAll(Type serviceType) {
			return this.container.LocateAll(serviceType);
		}
	}
}
