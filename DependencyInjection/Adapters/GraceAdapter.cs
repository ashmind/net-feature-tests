using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
	public class GraceAdapter : ContainerAdapterBase {
		private readonly DependencyInjectionContainer container = new DependencyInjectionContainer { ThrowExceptions = true };
	    private IInjectionScope webRequestScope;

	    public override Assembly Assembly {
			get { return typeof(DependencyInjectionContainer).Assembly; }
		}

		public override void RegisterTransient(Type serviceType, Type implementationType) {
			this.container.Configure(c => c.Export(implementationType).As(serviceType).AutoWireProperties());
		}

		public override void RegisterSingleton(Type serviceType, Type implementationType) {
			this.container.Configure(c => c.Export(implementationType).As(serviceType).AndSingleton());
		}

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.container.Configure(c => c.Export(implementationType).As(serviceType).AndSingletonPerScope());
        }

		public override void RegisterInstance(Type serviceType, object instance) {
			this.container.Configure(c => c.ExportInstance(instance).As(serviceType));
		}

        public override void AfterBeginWebRequest() {
            this.webRequestScope = this.container.CreateChildScope();
        }

        public override void BeforeEndWebRequest() {
            this.webRequestScope.Dispose();
            this.webRequestScope = null;
        }

		public override object Resolve(Type serviceType) {
		    if (this.webRequestScope != null)
		        return this.webRequestScope.Locate(serviceType);

			return this.container.Locate(serviceType);
		}

		public override IEnumerable<object> ResolveAll(Type serviceType) {
            if (this.webRequestScope != null)
                return this.webRequestScope.LocateAll(serviceType);

			return this.container.LocateAll(serviceType);
		}
	}
}
