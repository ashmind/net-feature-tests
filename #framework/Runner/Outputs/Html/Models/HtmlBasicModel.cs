using System.Collections.Generic;

namespace FeatureTests.Runner.Outputs.Html.Models {
    public class HtmlBasicModel {
        public HtmlBasicModel() {
            this.Navigation = new List<NavigationLinkModel>();
        }

        public IList<NavigationLinkModel> Navigation { get; private set; }
        public dynamic Labels                        { get; set; }
    }
}