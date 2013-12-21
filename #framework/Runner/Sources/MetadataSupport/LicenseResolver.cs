using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    public class LicenseResolver {
        #region LicenseRoot Class
        private class LicenseRoot {
            public LicenseInfo[] Licenses { get; set; } 
        }
        #endregion

        private readonly HttpDataProvider httpProvider;
        private readonly IList<LicenseInfo> licenses;

        public LicenseResolver(HttpDataProvider httpProvider, Uri licensesJsonUrl) {
            this.httpProvider = httpProvider;

            var licensesJson = this.httpProvider.GetStringAsync(licensesJsonUrl).Result;
            this.licenses = JsonConvert.DeserializeObject<LicenseRoot>(licensesJson).Licenses;
        }

        public async Task<LicenseInfo> GetLicenseInfo(Uri licenseUrl) {
            var licenseInfo = this.licenses.FirstOrDefault(l => l.Urls.Contains(licenseUrl));
            if (licenseInfo != null)
                return licenseInfo;

            var licenseText = await httpProvider.GetStringAsync(licenseUrl);
            licenseText = Regex.Replace(licenseText, "<[^>]+>", ""); // in case it is HTML -- naive but let's not overengineer
            licenseInfo = this.licenses.Where(l => l.Pattern != null)
                                       .FirstOrDefault(l => IsMatch(l, licenseText));

            return licenseInfo;
        }

        private bool IsMatch(LicenseInfo licenseInfo, string licenseText) {
            var pattern = Regex.Replace(licenseInfo.Pattern, @"\s+", "\\s+");
            return Regex.IsMatch(licenseText, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
    }
}