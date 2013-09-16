using System;

namespace DependencyInjection.FeatureTables.Generator.Data {
    public class FeatureCell {
        public string DisplayText { get; set; }
        public Uri DisplayUri { get; set; }
        public string Comment { get; set; }
        public FeatureState State { get; set; }

        public bool HasComment {
            get { return !string.IsNullOrEmpty(this.Comment); }
        }

        public string RawError { get; set; }
    }
}