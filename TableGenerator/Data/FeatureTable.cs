using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AshMind.Extensions;
using DependencyInjection.FeatureTests.Adapters;

namespace DependencyInjection.TableGenerator.Data {
    public class FeatureTable {
        private readonly IDictionary<Tuple<string, string>, FeatureCell> cells;

        public FeatureTable(string name, IEnumerable<IFrameworkAdapter> frameworks, IEnumerable<string> featureNames) {
            this.Name = name;
            this.Frameworks = (frameworks as IList<IFrameworkAdapter> ?? frameworks.ToList()).AsReadOnly();
            this.FeatureNames = (featureNames as IList<string> ?? featureNames.ToList()).AsReadOnly();
            this.cells = (
                from framework in this.Frameworks
                from featureName in this.FeatureNames
                select Tuple.Create(framework.FrameworkName, featureName)
            ).ToDictionary(t => t, t => new FeatureCell());
        }

        public string Name { get; private set; }
        public ReadOnlyCollection<IFrameworkAdapter> Frameworks { get; private set; }
        public ReadOnlyCollection<string> FeatureNames { get; private set; }

        public FeatureCell this[IFrameworkAdapter framework, string featureName] {
            get { return this.cells[Tuple.Create(framework.FrameworkName, featureName)]; }
        }

        public IEnumerable<Tuple<IFrameworkAdapter, IEnumerable<FeatureCell>>> GetRows() {
            return from framework in this.Frameworks
                   select Tuple.Create(framework, from featureName in this.FeatureNames select this[framework, featureName]);
        }
    }
}