using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;
using Newtonsoft.Json;

namespace FeatureTests.Shared.ResultData {
    public class FeatureTable {
        private readonly IReadOnlyDictionary<Tuple<string, object>, FeatureCell> cells;

        public FeatureTable(string displayName, IEnumerable<ILibrary> libraries, IEnumerable<Feature> features)
            : this(new object(), displayName, libraries, features)
        {
        }

        public FeatureTable(object key, string displayName, IEnumerable<ILibrary> libraries, IEnumerable<Feature> features) {
            this.Key = key;
            this.DisplayName = displayName;
            this.Libraries = (libraries as IList<ILibrary> ?? libraries.ToList()).AsReadOnly();
            this.Features = (features as IList<Feature> ?? features.ToList()).AsReadOnly();
            this.Scoring = FeatureScoring.PointPerFeature;
            this.cells = (
                from adapter in this.Libraries
                from feature in this.Features
                select Tuple.Create(adapter.Name, feature.Key)
            ).ToDictionary(t => t, t => new FeatureCell());
        }

        public object Key { get; private set; }
        public string DisplayName { get; private set; }
        public FeatureScoring Scoring { get; set; }
        public string Description { get; set; }
        public IReadOnlyCollection<ILibrary> Libraries { get; private set; }
        public IReadOnlyCollection<Feature> Features { get; private set; }

        public FeatureCell this[ILibrary library, Feature feature] {
            get { return this[library, feature.Key]; }
        }

        public FeatureCell this[ILibrary library, object featureKey] {
            get { return this.cells[Tuple.Create(library.Name, featureKey)]; }
        }

        public IEnumerable<Tuple<ILibrary, IEnumerable<FeatureCell>>> GetRows() {
            return from library in this.Libraries
                   select Tuple.Create(library, from feature in this.Features select this[library, feature]);
        }

        public int GetScore(ILibrary library) {
            if (this.Scoring == FeatureScoring.NotScored)
                return 0;

            if (this.Scoring == FeatureScoring.SinglePoint)
                return this.Features.Any(f => this[library, f].State == FeatureState.Success) ? 1 : 0;

            return this.Features.Count(f => this[library, f].State == FeatureState.Success);
        }

        public int MaxScore {
            get {
                if (this.Scoring == FeatureScoring.NotScored)
                    return 0;

                if (this.Scoring == FeatureScoring.SinglePoint)
                    return 1;

                return this.Features.Count;
            }
        }
    }
}