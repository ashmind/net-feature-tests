using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace DependencyInjection.FeatureTables.Generator.Sources.MetadataSupport {
    public class MetadataPackageCache {
        private readonly IPackageRepository packageRepository;
        private readonly ConcurrentDictionary<string, IPackage> packages = new ConcurrentDictionary<string, IPackage>();

        public MetadataPackageCache(string nugetPackagesPath) {
            this.packageRepository = new SharedPackageRepository(nugetPackagesPath);
        }

        public IPackage GetPackage(string packageId) {
            return packages.GetOrAdd(packageId, _ => {
                var package = packageRepository.FindPackagesById(packageId).SingleOrDefault();
                if (package == null)
                    throw new InvalidOperationException("Package '" + packageId + "' was not found in '" + this.packageRepository.Source + "'.");

                return package;
            });
        }
    }
}
