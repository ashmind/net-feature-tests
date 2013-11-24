using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AshMind.Extensions;
using Newtonsoft.Json;
using RazorTemplates.Core;
using FeatureTests.Runner.Outputs.Html;

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

        public void Write(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            this.RenderTemplate(arguments, allArgumentsForThisRun);
            this.CopyFiles(templatesDirectory, arguments.OutputDirectory, f => FileExtensionsToCopy.Contains(f.Extension));

            if (arguments.KeepUpdatingIfTemplatesChange)
                this.KeepUpdatingFrom(arguments, allArgumentsForThisRun);
        }

        private void KeepUpdatingFrom(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            if (this.watcher == null)
                this.watcher = new FileSystemWatcher(this.templatesDirectory.FullName, TemplateFileName);

            this.watcher.Changed += (sender, e) => {
                if (e.ChangeType != WatcherChangeTypes.Changed)
                    return;

                try {
                    this.RenderTemplate(arguments, allArgumentsForThisRun);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            };
            this.watcher.EnableRaisingEvents = true;
        }

        private void RenderTemplate(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            var targetPath = Path.Combine(arguments.OutputDirectory.FullName, arguments.OutputNamePrefix + ".html");
            try {
                var templateSource = File.ReadAllText(this.templatePath);
                var template = Template.WithBaseType<HtmlTemplateBase<HtmlResultModel>>()
                                       .Compile<HtmlResultModel>(templateSource);


                var model = this.GetModel(arguments, allArgumentsForThisRun);
                var templateResult = template.Render(model);
                
                File.WriteAllText(targetPath, templateResult);
                ConsoleEx.WriteLine(ConsoleColor.DarkGray, "    rendered " + targetPath);
            }
            catch (Exception ex) {
                File.WriteAllText(targetPath, ex.ToString());
                throw;
            }
        }

        private HtmlResultModel GetModel(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            var labels = this.GetLabels(arguments);
            var links = allArgumentsForThisRun.Select(a => {
                var url = a.OutputNamePrefix + ".html";
                var name = (string)GetLabels(a).LinkTitle;

                return new HtmlNavigationLink(url, name) {
                    IsCurrent = (a == arguments)
                };
            }).ToArray();

            return new HtmlResultModel(arguments.Tables, links) {
                HtmlAfterAll = this.GetResource(arguments.Assembly, "AfterAll.html"),
                Labels = labels
            };
        }

        private dynamic GetLabels(ResultOutputArguments arguments) {
            var labelsString = this.GetResource(arguments.Assembly, "Labels.json");
            return JsonConvert.DeserializeObject(labelsString.NullIfEmpty() ?? "{}");
        }

        private string GetResource(Assembly assembly, string name) {
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".Html." + name));
            if (resourceName == null)
                return "";

            ConsoleEx.WriteLine(ConsoleColor.DarkGray, "    found " + resourceName);

            using (var stream = assembly.GetManifestResourceStream(resourceName)) {
                return new StreamReader(stream).ReadToEnd();
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

                ConsoleEx.WriteLine(ConsoleColor.DarkGray, "    hardlinked " + targetPath);
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
