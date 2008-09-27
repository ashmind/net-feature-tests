using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC.Framework.Tests.Adapters {
    public interface IFrameworkAdapter {
        void RegisterSingleton<TComponent, TService>()
            where TComponent : TService;
        void RegisterTransient<TComponent, TService>()
            where TComponent : TService;

        void Register<TService>(TService instance);

        // Not tested yet, since attribute-based registration does not seem like a best way to provide feature.
        // And convention-based registration is too framework-specific.
        void RegisterAll(Assembly assembly);

        /// <summary>For open generics testing.</summary>
        void Register(Type componentType, Type serviceType);

        TService Resolve<TService>();
        TService Resolve<TService>(IDictionary<string, object> additionalArguments);
        TComponent Create<TComponent>();

        /// <summary>
        /// This test was run only once because there is no way to recover from StackOverflowException.
        /// </summary>
        bool CrashesOnRecursion { get; }
    }
}
