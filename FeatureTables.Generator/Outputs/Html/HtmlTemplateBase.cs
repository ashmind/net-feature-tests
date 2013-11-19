using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using DependencyInjection.FeatureTables.Generator.Data;
using MarkdownSharp;
using RazorTemplates.Core;
using RazorTemplates.Core.Infrastructure;

namespace DependencyInjection.FeatureTables.Generator.Outputs.Html {
    public abstract class HtmlTemplateBase<TModel> : TemplateBase<TModel> {
        private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo {
            NumberGroupSeparator = " "
        };

        private readonly Markdown markdown;

        protected HtmlTemplateBase() {
            this.markdown = new Markdown(new MarkdownOptions { AutoHyperlink = true });
        }

        public new TModel Model {
            get { return (TModel)base.Model; }
        }

        protected string GenerateAnchor(string displayName) {
            var result = displayName;
            result = Regex.Replace(result, @"\([^)]+\)|\<[^>]+\>", "");
            result = Regex.Replace(result, @"(?<=\W|$)\w", m => m.Value.ToUpperInvariant());
            result = Regex.Replace(result, @"\W+", "");

            return result;
        }

        protected string GetCssClassesForCell(FeatureCell cell) {
            var classes = cell.State.ToString().ToLowerInvariant();

            if (cell.DisplayValue is int) {
                classes += " number";
            }
            
            return classes;
        }

        protected string FormatDisplayValue(object value) {
            if (value == null)
                return "";

            if (value is string)
                return (string)value;

            if (value is int) {
                return ((int)value).ToString("N0", NumberFormat);
            }

            if (value is DateTimeOffset) {
                return ((DateTimeOffset)value).ToString("dd.MM.yyyy");
            }

            throw new NotSupportedException(string.Format("Value '{0}' ({1}) is not supported.", value, value.GetType()));
        }

        protected IHtmlString FormatDescription(string text) {
            var formatted = this.markdown.Transform(text);
            // also link twitter usernames
            formatted = Regex.Replace(formatted, @"(?<=[^\w\d])@([\w\d]+)", "<a href='http://www.twitter.com/$1'>@$1</a>");

            return new HtmlString(formatted);
        }

        protected new void Write(object value) {
            if (value == null)
                return;

            if (!(value is IHtmlString))
                value = WebUtility.HtmlEncode(value.ToString());

            base.Write(value);
        }

        protected new void WriteAttribute(string attribute, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values) {
            base.Write(prefix.Value);
            if (values != null) {
                foreach (var attributeValue in values) {
                    base.Write(attributeValue.Prefix.Value);
                    var obj = attributeValue.Value.Value;
                    this.Write(obj);
                }
            }
            base.Write(suffix.Value);
        }
    }
}
