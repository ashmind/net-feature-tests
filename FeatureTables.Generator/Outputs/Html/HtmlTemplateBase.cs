using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RazorTemplates.Core;
using RazorTemplates.Core.Infrastructure;

namespace DependencyInjection.FeatureTables.Generator.Outputs.Html {
    public abstract class HtmlTemplateBase<TModel> : TemplateBase<TModel> {
        public new TModel Model {
            get { return (TModel)base.Model; }
        }

        protected new void Write(object value) {
            if (value == null)
                return;

            base.Write(WebUtility.HtmlEncode(value.ToString()));
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
