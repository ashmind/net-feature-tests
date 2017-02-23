using System;
using System.Collections.Generic;
using System.Reflection;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using Stashbox;
using Stashbox.Configuration;
using Stashbox.Infrastructure;
using Stashbox.Web.Mvc;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class StashboxAdapter : ContainerAdapterBase {
        private readonly IStashboxContainer container;
        private readonly StashboxPerRequestScopeProvider perRequestScope;

        public StashboxAdapter() {
            this.container = new StashboxContainer(config => config
                .WithCircularDependencyTracking()
                .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)
                .WithDisposableTransientTracking()
                .WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjection.PropertiesWithPublicSetter)
                .WithOptionalAndDefaultValueInjection()
                .WithParentContainerResolution()
                .WithUnknownTypeResolution()
                .WithCircularDependencyWithLazy());

            this.perRequestScope = new StashboxPerRequestScopeProvider(this.container);
        }

        public override Assembly Assembly => typeof(StashboxContainer).Assembly;

        public override void RegisterInstance(Type serviceType, object instance) =>
            this.container.PrepareType(serviceType).WithInstance(instance).Register();

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) =>
            this.container.PrepareType(serviceType, implementationType).WithLifetime(new PerRequestLifetime()).Register();

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
