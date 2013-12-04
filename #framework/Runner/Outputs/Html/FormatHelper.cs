using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using FeatureTests.Shared.ResultData;
using MarkdownSharp;

namespace FeatureTests.Runner.Outputs.Html {
    public class FormatHelper {
        private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo {
            NumberGroupSeparator = " "
        };

        private readonly Markdown markdown;

        public FormatHelper() {
            this.markdown = new Markdown(new MarkdownOptions { AutoHyperlink = true });
        }

        public string GetCssClassesForCell(FeatureCell cell) {
            var classes = cell.State.ToString().ToLowerInvariant();

            if (cell.DisplayValue is int)
                classes += " number";

            if (cell.DisplayValue is DateTimeOffset)
                classes += " date";

            return classes;
        }

        public string DisplayValue(object value) {
            if (value == null)
                return "";

            if (value is string)
                return (string)value;

            if (value is int)
                return ((int)value).ToString("N0", NumberFormat);

            if (value is DateTimeOffset)
                return ((DateTimeOffset)value).ToString("MMM yyyy", CultureInfo.InvariantCulture).ToLowerInvariant();

            throw new NotSupportedException(String.Format("Value '{0}' ({1}) is not supported.", value, value.GetType()));
        }

        public IHtmlString Description(string text) {
            var formatted = this.markdown.Transform(text);
            // also link twitter usernames
            formatted = Regex.Replace(formatted, @"(?<=[^\w\d])@([\w\d]+)", "<a href='http://www.twitter.com/$1'>@$1</a>");

            return new HtmlString(formatted);
        }
    }
}