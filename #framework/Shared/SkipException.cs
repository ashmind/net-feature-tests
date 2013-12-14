using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FeatureTests.Shared {
    [Serializable]
    public class SkipException : Exception {
        public SkipException() {}
        public SkipException(string message) : base(message) {}
        public SkipException(string message, Exception inner) : base(message, inner) {}
        protected SkipException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
