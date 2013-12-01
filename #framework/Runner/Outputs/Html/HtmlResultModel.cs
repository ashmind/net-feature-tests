using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs.Html {
    public class HtmlResultModel {
        public HtmlResultModel(IReadOnlyList<FeatureTable> tables, IReadOnlyList<NavigationLinkModel> navigationLinks) {
            this.Tables = tables;
            this.NavigationLinks = navigationLinks;
        }

        public IReadOnlyList<FeatureTable> Tables { get; private set; }
        public IReadOnlyList<NavigationLinkModel> NavigationLinks { get; private set; }
        
        public dynamic Labels                     { get; set; }
        public string HtmlAfterAll                { get; set; }
    }
}
