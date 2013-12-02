using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownSharp;
using RazorTemplates.Core;
using RazorTemplates.Core.Infrastructure;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs.Html {
    public abstract class HtmlTemplateBase<TModel> : TemplateBase<TModel> {
        private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo {
            NumberGroupSeparator = " "
        };

        private readonly Markdown markdown;

        protected HtmlTemplateBase() {
            this.markdown = new Markdown(new MarkdownOptions { AutoHyperlink = true });
        }

        public new TModel Model {
            get { return base.Model; }
        }

        protected string GetCssClassesForCell(FeatureCell cell) {
            var classes = cell.State.ToString().ToLowerInvariant();

            if (cell.DisplayValue is int)
                classes += " number";

            if (cell.DisplayValue is DateTimeOffset)
                classes += " date";
            
            return classes;
        }

        protected string FormatDisplayValue(object value) {
            if (value == null)
                return "";

            if (value is string)
                return (string)value;

            if (value is int)
                return ((int)value).ToString("N0", NumberFormat);

            if (value is DateTimeOffset)
                return ((DateTimeOffset)value).ToString("MMM yyyy", CultureInfo.InvariantCulture).ToLowerInvariant();

            throw new NotSupportedException(string.Format("Value '{0}' ({1}) is not supported.", value, value.GetType()));
        }

        protected IHtmlString FormatDescription(string text) {
            var formatted = this.markdown.Transform(text);
            // also link twitter usernames
            formatted = Regex.Replace(formatted, @"(?<=[^\w\d])@([\w\d]+)", "<a href='http://www.twitter.com/$1'>@$1</a>");

            return new HtmlString(formatted);
        }

        protected IHtmlString RawHtml(string html) {
            return new HtmlString(html);
        }

        protected new void Write(object value) {
            if (value == null)
                return;

            if (!(value is IHtmlString))
                value = WebUtility.HtmlEncode(value.ToString());

            base.Write(value);
        }

        protected new void WriteAttribute(string attribute, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values) {
            // skip null attributes
            if (values == null || values.All(v => v.Value.Value == null))
                return;

            // special processing for booleans
            if (values.Length == 1) {
                var value = values[0].Value.Value;
                if (value.Equals(false))
                    return;

                if (value.Equals(true)) {
                    base.Write(attribute);
                    return;
                }
            }

            base.Write(prefix.Value);
            foreach (var attributeValue in values) {
                base.Write(attributeValue.Prefix.Value);
                this.Write(attributeValue.Value.Value);
            }
            base.Write(suffix.Value);
        }
    }
}
