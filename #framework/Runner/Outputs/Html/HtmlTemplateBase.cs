using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using RazorTemplates.Core;
using RazorTemplates.Core.Infrastructure;

namespace FeatureTests.Runner.Outputs.Html {
    public abstract class HtmlTemplateBase<TModel> : TemplateBase<TModel> {
        protected HtmlTemplateBase() {
            this.Html = new HtmlHelper();
            this.Format = new FormatHelper();
        }

        protected HtmlHelper Html { get; private set; }
        protected FormatHelper Format { get; private set; }

        public string Body { get; set; }

        public new TModel Model {
            get { return base.Model; }
        }

        protected IHtmlString RenderBody() {
            return new HtmlString(this.Body);
        }

        protected new void Write(object value) {
            base.Write(HtmlEncode(value));
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
                //System.Diagnostics.Debugger.Launch();
                this.WriteAttributeValue(attributeValue.Value.Value);
            }
            base.Write(suffix.Value);
        }

        private void WriteAttributeValue(object value) {
            if (value == null)
                return;

            var encoded = HtmlEncode(value).Replace("&", "&amp;")
                                           .Replace("\"", "&quot;");
            base.Write(encoded);
        }

        private string HtmlEncode(object value) {
            if (value == null)
                return null;

            var html = value as IHtmlString;
            if (html != null)
                return html.ToHtmlString();
            
            return WebUtility.HtmlEncode(value.ToString());
        }
    }
}
