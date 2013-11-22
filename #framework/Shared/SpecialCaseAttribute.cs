using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Shared {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class SpecialCaseAttribute : Attribute {
        public Type AdapterType { get; private set; }
        public string Comment { get; private set; }

        /// <summary>
        /// Defines whether to skip tests for this <see cref="AdapterType" />.
        /// </summary>
        public bool Skip { get; set; }

        public SpecialCaseAttribute(Type adapterType, string comment) {
            this.AdapterType = adapterType;
            this.Comment = comment;
        }
    }
}
