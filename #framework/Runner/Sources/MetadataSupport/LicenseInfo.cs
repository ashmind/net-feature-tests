using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    public class LicenseInfo {
        public LicenseInfo() {
            this.Urls = new List<Uri>();
        }

        public string ShortName { get; set; }
        public string Pattern { get; set; }
        public IList<Uri> Urls { get; private set; }
    }
}
