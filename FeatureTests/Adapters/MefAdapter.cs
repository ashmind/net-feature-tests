using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using DependencyInjection.FeatureTests.Adapters.Support;
using DependencyInjection.FeatureTests.Adapters.Support.GenericPlaceholders;
using NuGet;

namespace DependencyInjection.FeatureTests.Adapters {
    public class MefAdapter : FrameworkAdapterBase {
        private readonly CompositionContainer container;
        private readonly AggregateCatalog catalog = new AggregateCatalog();

        public MefAdapter() {
            this.container = new CompositionContainer(this.catalog);
        }

        public override string FrameworkName {
            get { return "MEF"; }
        }

        public override Assembly FrameworkAssembly {
            get { return typeof(RegistrationBuilder).Assembly; }
        }

        public override string FrameworkPackageId {
            get { return null; }
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            var builder = new RegistrationBuilder();
            builder.ForType(implementationType)
                   .Export(c => c.AsContractType(serviceType))
                   .SetCreationPolicy(CreationPolicy.NonShared);

            this.catalog.Catalogs.Add(new TypeCatalog(new[] { implementationType }, builder));
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            var builder = new RegistrationBuilder();
            builder.ForType(implementationType)
                   .Export(c => c.AsContractType(serviceType))
                   .SetCreationPolicy(CreationPolicy.Shared);

            this.catalog.Catalogs.Add(new TypeCatalog(new[] { implementationType }, builder));
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            GenericHelper.RewriteAndInvoke(() => this.container.ComposeExportedValue((X1)instance), serviceType);
        }

        public override object Resolve(Type serviceType) {
            return GenericHelper.RewriteAndInvoke(() => this.container.GetExportedValue<X1>(), serviceType);
        }
    }
}
