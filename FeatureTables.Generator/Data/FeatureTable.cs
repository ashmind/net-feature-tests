using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AshMind.Extensions;
using DependencyInjection.FeatureTests.Adapters;

namespace DependencyInjection.FeatureTables.Generator.Data {
    public class FeatureTable {
        private readonly IDictionary<Tuple<string, object>, FeatureCell> cells;

        public FeatureTable(string name, IEnumerable<IFrameworkAdapter> frameworks, IEnumerable<Feature> features) {
            this.Name = name;
            this.Frameworks = (frameworks as IList<IFrameworkAdapter> ?? frameworks.ToList()).AsReadOnly();
            this.Features = (features as IList<Feature> ?? features.ToList()).AsReadOnly();
            this.cells = (
                from framework in this.Frameworks
                from feature in this.Features
                select Tuple.Create(framework.FrameworkName, feature.Key)
            ).ToDictionary(t => t, t => new FeatureCell());
        }

        public string Name { get; private set; }
        public string Description { get; set; }
        public ReadOnlyCollection<IFrameworkAdapter> Frameworks { get; private set; }
        public ReadOnlyCollection<Feature> Features { get; private set; }

        public FeatureCell this[IFrameworkAdapter framework, Feature feature] {
            get { return this[framework, feature.Key]; }
        }

        public FeatureCell this[IFrameworkAdapter framework, object featureKey] {
            get { return this.cells[Tuple.Create(framework.FrameworkName, featureKey)]; }
        }

        public IEnumerable<Tuple<IFrameworkAdapter, IEnumerable<FeatureCell>>> GetRows() {
            return from framework in this.Frameworks
                   select Tuple.Create(framework, from feature in this.Features select this[framework, feature]);
        }
    }
}