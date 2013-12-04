using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;

namespace FeatureTests.Runner.Outputs.Html.Models {
    public class NavigationLinkModel {
        public NavigationLinkModel(string name, string url, bool onCurrentPage, IEnumerable<NavigationLinkModel> childLinks = null) {
            this.Name = name;
            this.Url = url;
            this.OnCurrentPage = onCurrentPage;
            this.ChildLinks = childLinks.EmptyIfNull().ToList();
        }

        public string Url         { get; set; }
        public string Name        { get; set; }
        public bool OnCurrentPage { get; set; }
        public IList<NavigationLinkModel> ChildLinks { get; private set; } 
    }
}