using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Grace.DependencyInjection;

namespace DependencyInjection.FeatureTests.Adapters
{
	public class GraceAdapter : FrameworkAdapterBase {
		private readonly DependencyInjectionContainer container = new DependencyInjectionContainer { ThrowExceptions = true };

		public override Assembly FrameworkAssembly {
			get { return typeof(DependencyInjectionContainer).Assembly; }
		}

		public override void RegisterTransient(Type serviceType, Type implementationType) {
			container.Configure(c => c.Export(implementationType).As(serviceType).AutoWireProperties());
		}

		public override void RegisterSingleton(Type serviceType, Type implementationType) {
			container.Configure(c => c.Export(implementationType).As(serviceType).AndSingleton());
		}

		public override void RegisterInstance(Type serviceType, object instance) {
			container.Configure(c => c.ExportInstance(instance).As(serviceType));
		}

		public override object Resolve(Type serviceType) {
			return container.Locate(serviceType);
		}

		public override IEnumerable<object> ResolveAll(Type serviceType) {
			return container.LocateAll(serviceType);
		}
	}
}
