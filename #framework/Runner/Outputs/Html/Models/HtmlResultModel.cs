using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs.Html.Models {
    public class HtmlResultModel : HtmlBasicModel {
        public HtmlResultModel(IReadOnlyList<FeatureTable> tables) {
            this.Tables = tables.ToList();
            this.TableIdMap = new Dictionary<FeatureTable, string>();
        }

        public IList<FeatureTable> Tables { get; private set; }
        public IDictionary<FeatureTable, string> TableIdMap { get; set; }

        public string HtmlBeforeAll { get; set; }
        public string HtmlAfterAll  { get; set; }
        public bool TotalVisible { get; set; }
    }
}
