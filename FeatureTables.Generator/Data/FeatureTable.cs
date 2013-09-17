using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;
using DependencyInjection.FeatureTests.Adapters;

namespace DependencyInjection.FeatureTables.Generator.Data {
    public class FeatureTable {
        private readonly IDictionary<Tuple<string, object>, FeatureCell> cells;

        public FeatureTable(string displayName, IEnumerable<IFrameworkAdapter> frameworks, IEnumerable<Feature> features) 
            : this(new object(), displayName, frameworks, features)
        {
        }

        public FeatureTable(object key, string displayName, IEnumerable<IFrameworkAdapter> frameworks, IEnumerable<Feature> features) {
            this.Key = key;
            this.DisplayName = displayName;
            this.Frameworks = (frameworks as IList<IFrameworkAdapter> ?? frameworks.ToList()).AsReadOnly();
            this.Features = (features as IList<Feature> ?? features.ToList()).AsReadOnly();
            this.Scoring = FeatureScoring.PointPerFeature;
            this.cells = (
                from framework in this.Frameworks
                from feature in this.Features
                select Tuple.Create(framework.FrameworkName, feature.Key)
            ).ToDictionary(t => t, t => new FeatureCell());
        }

        public object Key { get; private set; }
        public string DisplayName { get; private set; }
        public FeatureScoring Scoring { get; set; }
        public string Description { get; set; }
        public IReadOnlyCollection<IFrameworkAdapter> Frameworks { get; private set; }
        public IReadOnlyCollection<Feature> Features { get; private set; }

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

        public int GetScore(IFrameworkAdapter framework) {
            if (this.Scoring == FeatureScoring.NotScored)
                return 0;

            if (this.Scoring == FeatureScoring.PointPerTable)
                return this.Features.Any(f => this[framework, f].State == FeatureState.Success) ? 1 : 0;

            return this.Features.Count(f => this[framework, f].State == FeatureState.Success);
        }

        public int MaxScore {
            get {
                if (this.Scoring == FeatureScoring.NotScored)
                    return 0;

                if (this.Scoring == FeatureScoring.PointPerTable)
                    return 1;

                return this.Features.Count;
            }
        }
    }
}