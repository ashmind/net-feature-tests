using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace DependencyInjection.FeatureTests.Adapters
{
	public class GraceAdapter : FrameworkAdapterBase
	{
		private DependencyInjectionContainer container = new DependencyInjectionContainer { ThrowExceptions = true };

		public override Assembly FrameworkAssembly
		{
			get { return typeof(DependencyInjectionContainer).Assembly; }
		}

		public override void RegisterTransient(Type serviceType, Type implementationType)
		{
			container.Configure(c => c.Export(implementationType).As(serviceType).AutoWireProperties());
		}

		public override void RegisterInstance(Type serviceType, object instance)
		{
			container.Configure(c => c.ExportInstance(instance).As(serviceType));
		}

		public override object Resolve(Type serviceType)
		{

			//System.Console.WriteLine("Service: " + serviceType.FullName);
			var returnValue = container.Locate(serviceType);

			//System.Console.WriteLine("Found: " + returnValue.GetType().FullName);

			return returnValue;
		}

		public override void RegisterSingleton(Type serviceType, Type implementationType)
		{
			container.Configure(c => c.Export(implementationType).As(serviceType).AndSingleton());
		}

		public override IEnumerable<object> ResolveAll(Type serviceType)
		{
			return container.LocateAll(serviceType);
		}

		public override bool CrashesOnListRecursion
		{
			get
			{
				return false;
			}
		}

		public override bool CrashesOnRecursion
		{
			get
			{
				return false;
			}
		}
	}
}
