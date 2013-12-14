using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using FeatureTests.Shared;

namespace FeatureTests.On.DependencyInjection.Adapters.Interface {
    public interface IContainerAdapter : ILibrary {
        void RegisterSingleton(Type serviceType, Type implementationType);
        void RegisterTransient(Type serviceType, Type implementationType);
        
        // it is weird to have a special case for this one, as it is normally handled
        // by some kind of general subcontainer/scope support -- but for widest reach 
        // I want to start with the most common use case
        void RegisterPerWebRequest(Type serviceType, Type implementationType);
        
        void RegisterInstance(Type serviceType, object instance);

        void BeforeAllWebRequests(WebRequestTestHelper helper);
        void AfterBeginWebRequest();
        void BeforeEndWebRequest();

        object Resolve(Type serviceType);
        IEnumerable<object> ResolveAll(Type serviceType);
        
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
