using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace DependencyInjection.FeatureTests.Adapters {
    public interface IFrameworkAdapter : IServiceLocator {
        void RegisterSingleton(Type serviceType, Type componentType, string key);
        void RegisterTransient(Type serviceType, Type componentType, string key);
        void RegisterInstance(Type serviceType, object instance, string key);

        /// <summary>
        /// This test was run only once because there is no way to recover from StackOverflowException.
        /// </summary>
        bool CrashesOnRecursion     { get; }

        /// <summary>
        /// This test was run only once because there is no way to recover from StackOverflowException.
        /// </summary>
        bool CrashesOnListRecursion { get; }
    }
}
