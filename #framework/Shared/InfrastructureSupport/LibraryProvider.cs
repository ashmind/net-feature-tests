using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;

namespace FeatureTests.Shared.InfrastructureSupport {
    public static class LibraryProvider {
        private static readonly ConcurrentDictionary<Assembly, Type[]> cache = new ConcurrentDictionary<Assembly, Type[]>();

        public static IReadOnlyCollection<Type> GetAdapterTypes(Assembly assembly) {
            return cache.GetOrAdd(assembly,
                a => assembly.GetTypes()
                             .Where(t => t.HasInterface<ILibrary>() && !t.IsAbstract)
                             .OrderBy(t => t.Name)
                             .ToArray()
            );
        }

        public static IEnumerable<ILibrary> GetAdapters(Assembly assembly) {
            return GetAdapterTypes(assembly).Select(CreateAdapter);
        }

        public static ILibrary CreateAdapter(Type adapterType) {
            return (ILibrary)Activator.CreateInstance(adapterType);
        }
    }
}
