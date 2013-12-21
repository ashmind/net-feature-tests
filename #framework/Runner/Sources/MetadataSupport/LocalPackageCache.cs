using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    public class LocalPackageCache {
        private readonly IPackageRepository packageRepository;
        private readonly ConcurrentDictionary<string, IPackage> packages = new ConcurrentDictionary<string, IPackage>();

        public LocalPackageCache(string nugetPackagesPath) {
            this.packageRepository = new SharedPackageRepository(nugetPackagesPath);
        }

        public IPackage GetPackage(string packageId) {
            return this.packages.GetOrAdd(packageId, _ => {
                var found = this.packageRepository.FindPackagesById(packageId).ToArray();
                if (found.Length > 1) {
                    if (packageId == "ValueInjecter") { // special case/hack, remove when ValueInjecter is fixed
                        var correct = found.SingleOrDefault(p => p.Version.ToString() == "2.3.3");
                        if (correct != null)
                            return correct;
                    }

                    throw new Exception(string.Format("Found more than one package '{0}' in '{1}': check if any of those are obsolete/unused.", packageId, this.packageRepository.Source));
                }

                if (found.Length == 0)
                    throw new Exception(string.Format("Package '{0}' was not found in '{1}'.", packageId, this.packageRepository.Source));

                return found[0];
            });
        }
    }
}
