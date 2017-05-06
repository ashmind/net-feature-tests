using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using Stashbox;
using Stashbox.Infrastructure;
using Stashbox.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class StashboxAdapter : ContainerAdapterBase {
        private readonly IStashboxContainer container;
        private StashboxPerRequestScopeProvider perRequestScope;

        public StashboxAdapter() {
            this.container = new StashboxContainer(config => config
                .WithCircularDependencyTracking()
                .WithDisposableTransientTracking()
                .WithMemberInjectionWithoutAnnotation()
                .WithOptionalAndDefaultValueInjection()
                .WithUnknownTypeResolution()
                .WithCircularDependencyWithLazy());

            this.perRequestScope = new StashboxPerRequestScopeProvider(this.container);
        }

        public override Assembly Assembly => typeof(StashboxContainer).Assembly;

        public override string PackageId => "Stashbox";

        public override void RegisterInstance(Type serviceType, object instance) =>
            this.container.RegisterInstance(serviceType, instance);

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) =>
            this.container.RegisterType(serviceType, implementationType, context => context.WithLifetime(new PerRequestLifetime()));

        public override void RegisterSingleton(Type serviceType, Type implementationType) =>
            this.container.RegisterSingleton(serviceType, implementationType);

        public override void RegisterTransient(Type serviceType, Type implementationType) =>
            this.container.RegisterType(serviceType, implementationType);

        public override void AfterBeginWebRequest() =>
            this.perRequestScope.GetOrCreateScope();

        public override void BeforeAllWebRequests(WebRequestTestHelper helper) =>
            helper.RegisterModule<StashboxPerRequestLifetimeHttpModule>();

        public override object Resolve(Type serviceType) => this.container.Resolve(serviceType);

        public override IEnumerable<object> ResolveAll(Type serviceType) => this.container.ResolveAll(serviceType);
    }
}
