using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs.Html {
    public class HtmlResultModel {
        public HtmlResultModel(IReadOnlyList<FeatureTable> tables, string afterAll) {
            this.Tables = tables;
            this.AfterAll = afterAll;
        }

        public IReadOnlyList<FeatureTable> Tables { get; private set; }
        public string AfterAll                     { get; private set; }
    }
}
