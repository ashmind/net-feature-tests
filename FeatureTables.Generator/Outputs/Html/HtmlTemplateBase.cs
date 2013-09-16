using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownSharp;
using RazorTemplates.Core;
using RazorTemplates.Core.Infrastructure;

namespace DependencyInjection.FeatureTables.Generator.Outputs.Html {
    public abstract class HtmlTemplateBase<TModel> : TemplateBase<TModel> {
        private readonly Markdown markdown;

        protected HtmlTemplateBase() {
            this.markdown = new Markdown(new MarkdownOptions { AutoHyperlink = true });
        }

        public new TModel Model {
            get { return (TModel)base.Model; }
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
