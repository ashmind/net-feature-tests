using System;

namespace DependencyInjection.TableGenerator.Data {
    public class FeatureCell {
        public string Text { get; set; }
        public string Comment { get; set; }
        public FeatureState State { get; set; }
        public Exception Exception { get; set; }

        public bool HasComment {
            get { return !string.IsNullOrEmpty(this.Comment); }
        }
    }
}