using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Runner.Outputs.Html {
    public class HtmlNavigationLink {
        public HtmlNavigationLink(string url, string name) {
            this.Url = url;
            this.Name = name;
        }

        public string Url     { get; set; }
        public string Name    { get; set; }
        public bool IsCurrent { get; set; }
    }
}