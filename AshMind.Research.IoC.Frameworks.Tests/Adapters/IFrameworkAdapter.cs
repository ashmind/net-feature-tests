using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Research.IoC.Frameworks.Tests.Adapters {
    public interface IFrameworkAdapter {
        void RegisterSingleton<TComponent, TService>()
            where TComponent : TService;
        void RegisterTransient<TComponent, TService>()
            where TComponent : TService;

        void Register<TService>(TService instance);

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
