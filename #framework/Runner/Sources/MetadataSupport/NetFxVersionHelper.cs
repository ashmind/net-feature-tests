using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using NuGet;

namespace FeatureTests.Runner.Sources.MetadataSupport {
    // in real application it should not be static or helper
    public static class NetFxVersionHelper {

        public static IEnumerable<FrameworkName> Split(FrameworkName version) {
            if (!version.IsPortableFramework()) {
                yield return version;
                yield break;
            }

            var portable = NetPortableProfile.Parse(version.Profile);
            if (portable == null) {
                yield return version;
                yield break;
            }

            foreach (var supportedVersion in portable.SupportedFrameworks) {
                yield return supportedVersion;
            }
        }

        public static bool ShouldDisplay(FrameworkName version) {
            var normalized = VersionUtility.GetShortFrameworkName(version);
            return !normalized.StartsWith("portable");
        }

        public static string GetDisplayOrder(FrameworkName version) {
            var normalized = VersionUtility.GetShortFrameworkName(version);
            var order = "";
            if (normalized.StartsWith("net")) {
                order = "1-";
            }
            else if (normalized.StartsWith("win")) {
                order = "2-";
            }
            else if (normalized.StartsWith("wpa")) {
                order = "4-";
            }
            else if (normalized.StartsWith("uap", StringComparison.OrdinalIgnoreCase)) {
                order = "5-";
            }
            else if (normalized.StartsWith("wp")) {
                order = "6-";
            }
            else if (normalized.StartsWith("sl")) {
                order = "7-";
            }
            else {
                order = "8-" + normalized;
            }

            order += version.Version.Major + "." + version.Version.Minor;
            return order;
        }

        public static string GetDisplayName(FrameworkName version) {
            var normalized = VersionUtility.GetShortFrameworkName(version);

            // HACKS :)
            if (version.Profile.Equals("WindowsPhone", StringComparison.InvariantCultureIgnoreCase)
             && version.Identifier.Equals("Silverlight", StringComparison.InvariantCultureIgnoreCase)
             && version.Version.Major == 3) {
                return "Windows Phone 7";
            }

            // MORE HACKS
            var result = normalized;
            result = Regex.Replace(result, @"^net(?=\d)",          ".NET");
            result = Regex.Replace(result, @"(\d+(?:\.\d+)*)-cf$", " CF $1");
            result = Regex.Replace(result, @"^(sl\d)\d$",          "$1"); // Silverlight normally uses one digit
            result = Regex.Replace(result, @"^sl(?=\d)",           "Silverlight ");
            result = Regex.Replace(result, @"^wp(?=\d)",           "Windows Phone");
            result = Regex.Replace(result, @"^wp$",                "Windows Phone");
            result = Regex.Replace(result, @"^wpa$",               "Windows Phone App");
            result = Regex.Replace(result, @"^win(dows)?(8(0)?)?$", "Windows 8", RegexOptions.ExplicitCapture);
            result = Regex.Replace(result, @"^win81$",             "Windows 8.1");
            result = Regex.Replace(result, @"^wpa81$",             "Windows Universal (8.1)");
            result = Regex.Replace(result, @"^uap",                "Universal Windows Platform ", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"^xamarinios",         "Xamarin iOS");
            result = Regex.Replace(result, @"\d{2,}(?!\.)",        match => " " + string.Join(".", match.Value.ToCharArray())); // 45 => 4.5, etc
            result = Regex.Replace(result, @"-Client",             " (Client Profile)");

            return result;
        }
    }
}
