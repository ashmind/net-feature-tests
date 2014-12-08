using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using CacheCow.Common;
using Newtonsoft.Json;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    public class FlatFileCacheStore : ICacheStore {
        private readonly DirectoryInfo directory;
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private const string MetaSuffix = ".meta";
        private const string BodySuffix = ".body";

        public FlatFileCacheStore(DirectoryInfo directory) {
            this.directory = directory;
            if (!this.directory.Exists)
                this.directory.Create();
        }

        public void AddOrUpdate(CacheKey key, HttpResponseMessage response) {
            try {
                cacheLock.EnterWriteLock();
                var path = GetBaseFilePath(key.ResourceUri);
                WriteMeta(path + MetaSuffix, response);
                WriteBody(path + BodySuffix, response);
            }
            finally {
                if (cacheLock.IsWriteLockHeld)
                    cacheLock.ExitWriteLock();
            }
        }

        public void Clear() {
            try {
                cacheLock.EnterWriteLock();
                this.directory.Delete(true);
                this.directory.Create();
            }
            finally {
                if (cacheLock.IsWriteLockHeld)
                    cacheLock.ExitWriteLock();
            }
        }

        public bool TryGetValue(CacheKey key, out HttpResponseMessage response) {
            try {
                cacheLock.EnterReadLock();
                var path = GetBaseFilePath(key.ResourceUri);
                if (!File.Exists(path + BodySuffix)) {
                    response = null;
                    return false;
                }

                response = new HttpResponseMessage { Content = ReadBody(path + BodySuffix) };
                ReadMeta(response, path + MetaSuffix);
                return true;
            }
            finally {
                if (cacheLock.IsReadLockHeld)
                    cacheLock.ExitReadLock();
            }
        }

        public bool TryRemove(CacheKey key) {
            try {
                cacheLock.EnterWriteLock();
                var path = GetBaseFilePath(key.ResourceUri);
                var existed = File.Exists(path + BodySuffix);
                File.Delete(path + BodySuffix);
                File.Delete(path + MetaSuffix);

                return existed;
            }
            finally {
                if (cacheLock.IsWriteLockHeld)
                    cacheLock.ExitWriteLock();
            }
        }

        private void WriteMeta(string path, HttpResponseMessage response) {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(path)) {
                serializer.Serialize(writer, new {
                    response.StatusCode,
                    Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value)
                });
            }
        }

        private void WriteBody(string path, HttpResponseMessage response) {
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            File.WriteAllBytes(path, bytes);
            response.Content = new ByteArrayContent(bytes);
        }

        private void ReadMeta(HttpResponseMessage response, string path) {
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader)) {
                var meta = DeserializeAnonymous(serializer, jsonReader, new {
                    StatusCode = (HttpStatusCode)0,
                    Headers = (Dictionary<string, IEnumerable<string>>)null
                });
                response.StatusCode = meta.StatusCode;
                response.Headers.Clear();
                foreach (var pair in meta.Headers) {
                    response.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
                }
            }
        }

        private HttpContent ReadBody(string path) {
            return new ByteArrayContent(File.ReadAllBytes(path));
        }

        // ReSharper disable once UnusedParameter.Local
        private T DeserializeAnonymous<T>(JsonSerializer serializer, JsonTextReader reader, T template) {
            return serializer.Deserialize<T>(reader);
        }

        private string GetBaseFilePath(string url) {
            var fileName = Regex.Replace(url, @"[^\w\-\d%#]+", "_");
            return Path.Combine(this.directory.FullName, fileName);
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }
}
