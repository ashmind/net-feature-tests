using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using CacheCow.Common;
using Newtonsoft.Json;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    public class FlatFileCacheStore : ICacheStore {
        private readonly DirectoryInfo directory;
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private const string HeadersSuffix = ".headers";
        private const string ContentSuffix = ".content";

        public FlatFileCacheStore(DirectoryInfo directory) {
            this.directory = directory;
            if (!this.directory.Exists)
                this.directory.Create();
        }

        public void AddOrUpdate(CacheKey key, HttpResponseMessage response) {
            try {
                cacheLock.EnterWriteLock();
                var path = GetBaseFilePath(key.ResourceUri);
                WriteHeaders(path + HeadersSuffix, response);
                WriteContent(path + ContentSuffix, response);
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
                if (!File.Exists(path + ContentSuffix)) {
                    response = null;
                    return false;
                }

                response = new HttpResponseMessage { Content = ReadContent(path + ContentSuffix) };
                ReadHeaders(response, path + HeadersSuffix);
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
                var existed = File.Exists(path + ContentSuffix);
                File.Delete(path + ContentSuffix);
                File.Delete(path + HeadersSuffix);

                return existed;
            }
            finally {
                if (cacheLock.IsWriteLockHeld)
                    cacheLock.ExitWriteLock();
            }
        }

        private void WriteHeaders(string path, HttpResponseMessage response) {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(path)) {
                serializer.Serialize(writer, response.Headers.ToDictionary(h => h.Key, h => h.Value));
            }
        }

        private void WriteContent(string path, HttpResponseMessage response) {
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            File.WriteAllBytes(path, bytes);
            response.Content = new ByteArrayContent(bytes);
        }

        private void ReadHeaders(HttpResponseMessage response, string path) {
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader)) {
                var dictionary = serializer.Deserialize<IDictionary<string, string[]>>(jsonReader);
                response.Headers.Clear();
                foreach (var pair in dictionary) {
                    response.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
                }
            }
        }

        private HttpContent ReadContent(string path) {
            return new ByteArrayContent(File.ReadAllBytes(path));
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
