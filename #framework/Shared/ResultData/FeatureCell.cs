using System;

namespace FeatureTests.Shared.ResultData {
    public class FeatureCell {
        public object DisplayValue { get; set; }
        public Uri DisplayUri { get; set; }
        public string Details { get; set; }
        public FeatureState State { get; set; }

        public bool HasDetails {
            get { return !string.IsNullOrEmpty(this.Details); }
        }

        public string RawError { get; set; }
    }
}