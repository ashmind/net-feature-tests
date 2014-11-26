using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    [Serializable]
    public class HttpDataRequestException : Exception {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpDataRequestException(HttpStatusCode statusCode) {
            StatusCode = statusCode;
        }

        public HttpDataRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpDataRequestException(string message, HttpStatusCode statusCode, Exception inner)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }

        protected HttpDataRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
