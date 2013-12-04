using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using FeatureTests.Runner.Outputs.Html.Models;
using FeatureTests.Shared.ResultData;
using Newtonsoft.Json;
using RazorTemplates.Core;
using FeatureTests.Runner.Outputs.Html;

namespace FeatureTests.Runner.Outputs {
    public class HtmlOutput : IResultOutput {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        private const string ResultTemplateFileName = "Results.cshtml";
        private const string LayoutTemplateFileName = "_Layout.cshtml";

        private static readonly ISet<string> FileExtensionsToCopy = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {
            ".css", ".js", ".html", ".htm"
        };

        private readonly DirectoryInfo templatesDirectory;
        private readonly string resultTemplatePath;
        private readonly string layoutTemplatePath;
        private FileSystemWatcher watcher;

        public HtmlOutput(DirectoryInfo templatesDirectory) {
            this.templatesDirectory = templatesDirectory;
            this.resultTemplatePath = Path.Combine(templatesDirectory.FullName, ResultTemplateFileName);
            this.layoutTemplatePath = Path.Combine(templatesDirectory.FullName, LayoutTemplateFileName);
        }

        public void Write(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            this.RenderTemplate(arguments, allArgumentsForThisRun);
            this.CopyFiles(templatesDirectory, arguments.OutputDirectory, f => FileExtensionsToCopy.Contains(f.Extension));

            if (arguments.KeepUpdatingIfTemplatesChange)
                this.KeepUpdatingFrom(arguments, allArgumentsForThisRun);
        }

        private void KeepUpdatingFrom(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            if (this.watcher == null)
                this.watcher = new FileSystemWatcher(this.templatesDirectory.FullName, "*.cshtml");

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
                var model = this.BuildModel(arguments, allArgumentsForThisRun);
                var templateBodyResult = this.RenderTemplateToStringSafe(this.resultTemplatePath, model);
                var templateLayoutResult = this.RenderTemplateToStringSafe<HtmlBasicModel>(this.layoutTemplatePath, model, m => m.Body = templateBodyResult);

                File.WriteAllText(targetPath, templateLayoutResult);
                ConsoleEx.WriteLine(ConsoleColor.DarkGray, "    rendered " + targetPath);
            }
            catch (Exception ex) {
                File.WriteAllText(targetPath, ex.ToString());
                throw;
            }
        }

        private string RenderTemplateToStringSafe<TModel>(string templatePath, TModel model, Action<HtmlTemplateBase<TModel>> initializer = null) 
            where TModel : HtmlBasicModel
        {
            initializer = initializer ?? (t => {});

            var templateSource = File.ReadAllText(templatePath);
            var template = Template.WithBaseType<HtmlTemplateBase<TModel>>((Action<TemplateBase>)(t => initializer((HtmlTemplateBase<TModel>)t)))
                                   .Compile<TModel>(templateSource);

            return template.Render(model);
        }

        private HtmlResultModel BuildModel(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            var labels = this.GetLabels(arguments);

            var model = new HtmlResultModel(arguments.Tables) {
                HtmlAfterAll = this.GetResource(arguments.Assembly, "AfterAll.html"),
                Labels = labels
            };
            BuildNavigationLinks(model.Navigation, arguments, allArgumentsForThisRun);
            foreach (var table in arguments.Tables) {
                model.TableIdMap.Add(table, this.GenerateTableId(table));
            }

            return model;
        }

        private void BuildNavigationLinks(IList<NavigationLinkModel> navigation, ResultOutputArguments currentArguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun) {
            foreach (var arguments in allArgumentsForThisRun) {
                var url = arguments.OutputNamePrefix + ".html";
                var name = (string)this.GetLabels(arguments).LinkTitle;
                var onCurrentPage = arguments == currentArguments;

                var link = new NavigationLinkModel(
                    name, url, onCurrentPage,
                    arguments.Tables.Select(t => new NavigationLinkModel(
                        t.DisplayName, url + "#" + this.GenerateTableId(t), onCurrentPage
                    )
                 ));

                navigation.Add(link);
            }
        }

        private string GenerateTableId(FeatureTable table) {
            var result = table.DisplayName;
            result = Regex.Replace(result, @"\<[^>]+\>", "");
            result = Regex.Replace(result, @"(?<=\W|$)\w", m => m.Value.ToUpperInvariant());
            result = Regex.Replace(result, @"\W+", "");

            return result;
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
