using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs.Html {
    public class HtmlResultModel {
        public HtmlResultModel(IReadOnlyList<FeatureTable> tables) {
            this.Tables = tables.ToList();
            this.TableIdMap = new Dictionary<FeatureTable, string>();
            this.Navigation = new List<NavigationLinkModel>();
        }

        public IList<FeatureTable> Tables { get; private set; }
        public IDictionary<FeatureTable, string> TableIdMap { get; set; }
        public IList<NavigationLinkModel> Navigation { get; private set; }
        
        public dynamic Labels                     { get; set; }
        public string HtmlAfterAll                { get; set; }
    }
}
