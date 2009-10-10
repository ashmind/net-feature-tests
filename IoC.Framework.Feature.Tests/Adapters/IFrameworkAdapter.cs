using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Abstraction;
using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Feature.Tests.Adapters {
    public interface IFrameworkAdapter : IServiceInjectionFramework, IServiceContainer, IServiceLocator, IComponentFactory {
        /// <summary>
        /// This test was run only once because there is no way to recover from StackOverflowException.
        /// </summary>
        bool CrashesOnRecursion     { get; }

        /// <summary>
        /// This test was run only once because there is no way to recover from StackOverflowException.
        /// </summary>
        bool CrashesOnListRecursion { get; }

        IComponentFactory CreateFactory(IServiceContainer container);
    }
}
