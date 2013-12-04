using System.Web;

namespace FeatureTests.Runner.Outputs.Html {
    public class HtmlHelper {
        public IHtmlString Raw(string html) {
            return new HtmlString(html);
        }
    }
}