using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
	public class GraceAdapter : AdapterBase {
		private readonly DependencyInjectionContainer container = new DependencyInjectionContainer { ThrowExceptions = true };

		public override Assembly Assembly {
			get { return typeof(DependencyInjectionContainer).Assembly; }
		}

		public override void RegisterTransient(Type serviceType, Type implementationType) {
			this.container.Configure(c => c.Export(implementationType).As(serviceType).AutoWireProperties());
		}

		public override void RegisterSingleton(Type serviceType, Type implementationType) {
			this.container.Configure(c => c.Export(implementationType).As(serviceType).AndSingleton());
		}

		public override void RegisterInstance(Type serviceType, object instance) {
			this.container.Configure(c => c.ExportInstance(instance).As(serviceType));
		}

		public override object Resolve(Type serviceType) {
			return this.container.Locate(serviceType);
		}

		public override IEnumerable<object> ResolveAll(Type serviceType) {
			return this.container.LocateAll(serviceType);
		}
	}
}
