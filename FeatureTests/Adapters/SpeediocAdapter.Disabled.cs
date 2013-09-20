using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Speedioc;
using Speedioc.Registration;
using Speedioc.Registration.Builders;

namespace DependencyInjection.FeatureTests.Adapters {
    //public class SpeediocAdapter : FrameworkAdapterBase {
    //    private IContainerBuilder builder = DefaultContainerBuilderFactory.GetInstance();
    //    private IContainer container;
        
    //    public override Assembly FrameworkAssembly {
    //        get { return typeof(DefaultContainerBuilderFactory).Assembly; }
    //    }

    //    // NOTE: Registration approach below is not the best approach.
    //    // As far as I understand, the best approach is to inherit Registry.
    //    // However that would not work for tests.

    //    public override void RegisterSingleton(Type serviceType, Type implementationType) {
    //        var registry = new Registry();
    //        registry.Register(implementationType).As(serviceType).WithLifetime(Lifetime.Container);
    //        this.builder.AddRegistry(registry);
    //    }

    //    public override void RegisterTransient(Type serviceType, Type implementationType) {
    //        var registry = new Registry();
    //        registry.Register(implementationType).As(serviceType).WithLifetime(Lifetime.Transient);
    //        this.builder.AddRegistry(registry);
    //    }

    //    public override void RegisterInstance(Type serviceType, object instance) {
    //        // How do I do that?
    //    }

    //    public override object Resolve(Type serviceType) {
    //        this.FreezeContainer();
    //        return this.container.GetInstance(serviceType);
    //    }

    //    private void FreezeContainer() {
    //        if (this.container != null)
    //            return;

    //        this.container = this.builder.Build();
    //        this.builder = null; // simple way to prevent accidental reuse of adapter
    //    }
    //}
}
