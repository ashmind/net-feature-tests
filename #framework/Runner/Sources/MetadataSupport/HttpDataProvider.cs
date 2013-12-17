using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CacheCow.Client;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    public class HttpDataProvider {
        private readonly DirectoryInfo cacheRoot;

        public HttpDataProvider(DirectoryInfo cacheRoot) {
            this.cacheRoot = cacheRoot;
        }

        public async Task<TResult> GetAsync<TResult>(string url) {
            using (var client = CreateHttpClient()) {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadAsAsync<TResult>();
            }
        }

        public async Task<string> GetStringAsync(string url) {
            using (var client = CreateHttpClient()) {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public HttpClient CreateHttpClient() {
            var handler = new CachingHandler(new FlatFileCacheStore(cacheRoot)) {
                InnerHandler = new HttpClientHandler()
            };
            var baseValidator = handler.ResponseValidator;
            handler.ResponseValidator = m => {
                var result = baseValidator(m);
                if (result != ResponseValidationResult.NotCacheable)
                    return result;

                var sinceName = "X-Cache-Forced-Since";
                var sinceString = m.Headers.Contains(sinceName)
                                ? m.Headers.GetValues(sinceName).SingleOrDefault()
                                : null;
                if (sinceString == null) {
                    m.Headers.Add(sinceName, DateTimeOffset.Now.ToString());
                    return ResponseValidationResult.OK;
                }

                var since = DateTimeOffset.Parse(sinceString);
                return (DateTimeOffset.Now - since).TotalHours <= 1
                     ? ResponseValidationResult.OK
                     : ResponseValidationResult.Stale;
            };

            return new HttpClient(handler);
        }
    }
}
