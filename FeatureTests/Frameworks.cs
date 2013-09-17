using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;
using DependencyInjection.FeatureTests.Adapters;

namespace DependencyInjection.FeatureTests {
    public static class Frameworks {
        private static readonly Type[] AdapterTypes =
            typeof(Frameworks).Assembly.GetTypes()
                                       .Where(t => t.HasInterface<IFrameworkAdapter>() && !t.IsAbstract)
                                       .OrderBy(t => t.Name)
                                       .ToArray();

        public static IEnumerable<IFrameworkAdapter> List() {
            return from type in AdapterTypes
                   select (IFrameworkAdapter)Activator.CreateInstance(type);
        }
    }
}
