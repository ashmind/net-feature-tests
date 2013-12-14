using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class MefAdapter : ContainerAdapterBase {
        private readonly CompositionContainer container;
        private readonly AggregateCatalog catalog = new AggregateCatalog();

        public MefAdapter() {
            this.container = new CompositionContainer(this.catalog);
        }

        public override string Name {
            get { return "MEF"; }
        }

        public override Assembly Assembly {
            get { return typeof(RegistrationBuilder).Assembly; }
        }

        public override string PackageId {
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

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBeProvided();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            GenericHelper.RewriteAndInvoke(() => this.container.ComposeExportedValue((X1)instance), serviceType);
        }

        public override object Resolve(Type serviceType) {
            return GenericHelper.RewriteAndInvoke(() => this.container.GetExportedValue<X1>(), serviceType);
        }
    }
}
