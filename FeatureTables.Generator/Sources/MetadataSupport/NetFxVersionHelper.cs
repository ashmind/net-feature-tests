using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using NuGet;

namespace DependencyInjection.FeatureTables.Generator.Sources.MetadataSupport {
    // in real application it should not be static or helper
    public static class NetFxVersionHelper {
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
            else if (normalized.StartsWith("wp")) {
                order = "3-";
            }
            else if (normalized.StartsWith("sl")) {
                order = "4-";
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
            result = Regex.Replace(result, @"^net(?=\d)",       ".NET ");
            result = Regex.Replace(result, @"^(sl\d)\d$",       "$1"); // Silverlight normally uses one digit
            result = Regex.Replace(result, @"^sl(?=\d)",        "Silverlight ");
            result = Regex.Replace(result, @"^wp(?=\d)",        "Windows Phone ");
            result = Regex.Replace(result, @"^wp$",             "Windows Phone");
            result = Regex.Replace(result, @"^win(dows)?(8(0)?)?$", "Windows 8", RegexOptions.ExplicitCapture);
            result = Regex.Replace(result, @"(\d)(\d)",         "$1.$2");
            result = Regex.Replace(result, @"-Client",          " (Client Profile)");

            return result;
        }
    }
}
