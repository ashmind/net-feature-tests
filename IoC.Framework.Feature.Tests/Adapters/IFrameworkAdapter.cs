using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Feature.Tests.Adapters {
    public interface IFrameworkAdapter : IServiceLocator {
        void AddSingleton(Type serviceType, Type componentType, string key);
        void AddTransient(Type serviceType, Type componentType, string key);
        void AddInstance(Type serviceType, object instance, string key);

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
