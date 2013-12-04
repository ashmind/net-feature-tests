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
