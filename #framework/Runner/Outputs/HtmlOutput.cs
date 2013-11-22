using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FeatureTests.Runner.Outputs.Html;
using RazorTemplates.Core;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs {
    public class HtmlOutput : IResultOutput {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        private const string TemplateFileName = "FeatureTests.cshtml";
        private static readonly ISet<string> FileExtensionsToCopy = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {
            ".css", ".js", ".html", ".htm"
        };

        private readonly DirectoryInfo templatesDirectory;
        private readonly string templatePath;
        private FileSystemWatcher watcher;

        public HtmlOutput(DirectoryInfo templatesDirectory) {
            this.templatesDirectory = templatesDirectory;
            this.templatePath = Path.Combine(templatesDirectory.FullName, TemplateFileName);
        }

        public void Write(IReadOnlyList<FeatureTable> tables, DirectoryInfo outputDirectory, string outputNamePrefix, bool keepUpdatingIfTemplatesChange = false) {
            this.RenderTemplate(tables, outputDirectory, outputNamePrefix);
            this.CopyFiles(templatesDirectory, outputDirectory, f => FileExtensionsToCopy.Contains(f.Extension));

            if (keepUpdatingIfTemplatesChange)
                this.KeepUpdatingFrom(tables, outputDirectory, outputNamePrefix);
        }

        private void KeepUpdatingFrom(IReadOnlyList<FeatureTable> tables, DirectoryInfo outputDirectory, string outputNamePrefix) {
            if (this.watcher == null)
                this.watcher = new FileSystemWatcher(this.templatesDirectory.FullName, TemplateFileName);

            this.watcher.Changed += (sender, e) => {
                if (e.ChangeType != WatcherChangeTypes.Changed)
                    return;

                try {
                    this.RenderTemplate(tables, outputDirectory, outputNamePrefix);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            };
            this.watcher.EnableRaisingEvents = true;
        }

        private void RenderTemplate(IReadOnlyList<FeatureTable> tables, DirectoryInfo outputDirectory, string outputNamePrefix) {
            var targetPath = Path.Combine(outputDirectory.FullName, outputNamePrefix + ".FeatureTests.html");
            try {
                var templateSource = File.ReadAllText(this.templatePath);
                var template = Template.WithBaseType<HtmlTemplateBase<IEnumerable<FeatureTable>>>()
                                       .Compile<IEnumerable<FeatureTable>>(templateSource);

                var templateResult = template.Render(tables);
                
                File.WriteAllText(targetPath, templateResult);
                Console.WriteLine("  Rendered " + targetPath);
            }
            catch (Exception ex) {
                File.WriteAllText(targetPath, ex.ToString());
                throw;
            }
        }

        public void CopyFiles(DirectoryInfo source, DirectoryInfo target, Func<FileInfo, bool> filter) {
            if (!target.Exists)
                target.Create();

            foreach (var file in source.EnumerateFiles()) {
                if (!filter(file))
                    continue;

                var targetPath = Path.Combine(target.FullName, file.Name);
                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                if (!CreateHardLink(targetPath, file.FullName, IntPtr.Zero))
                    throw new Exception("Failed to hardlink " + targetPath + ": " + Marshal.GetLastWin32Error() + ".");

                Console.WriteLine("  Hardlinked " + targetPath);
            }

            foreach (var sourceSubdirectory in source.EnumerateDirectories()) {
                CopyFiles(sourceSubdirectory, target.CreateSubdirectory(sourceSubdirectory.Name), filter);
            }
        }

        public void Dispose() {
            if (this.watcher != null)
                this.watcher.Dispose();
        }
    }
}
