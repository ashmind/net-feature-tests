using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AshMind.Extensions;

namespace FeatureTests.Runner.Sources.FeatureTestSupport {
    public class AttributeTextCleaner {
        public string CleanWhitespace(string attributeText) {
            if (attributeText.IsNullOrEmpty())
                return attributeText;

            // remove all spaces at the start of the line
            return Regex.Replace(attributeText, @"^ +", "", RegexOptions.Multiline);
        }
    }
}
