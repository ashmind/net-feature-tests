using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Runner.Outputs.Html {
    public class NavigationLinkModel {
        public NavigationLinkModel(string url, string name) {
            this.Url = url;
            this.Name = name;
            this.Children = new List<NavigationLinkModel>();
        }

        public string Url        { get; set; }
        public string Name       { get; set; }
        public bool IsTopLevelLinkToCurrentPage { get; set; }
        public IReadOnlyList<NavigationLinkModel> Children { get; private set; } 
    }
}