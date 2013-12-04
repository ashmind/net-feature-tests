using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using FeatureTests.Runner.Outputs.Html.Models;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using FeatureTests.Shared.ResultData;
using Newtonsoft.Json;
using RazorTemplates.Core;
using FeatureTests.Runner.Outputs.Html;

namespace FeatureTests.Runner.Outputs {
    public class HtmlOutput : IResultOutput {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        private const string LayoutTemplateFileName = "_Layout.cshtml";
        private const string ResultTemplateFileName = "Results.cshtml";

        private static readonly ISet<string> FileExtensionsToCopy = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {
            ".css", ".js", ".html", ".htm"
        };

        private readonly DirectoryInfo templatesDirectory;
        private FileSystemWatcher watcher;

        #region WorkingModel

        private class WorkingModel {
            public WorkingModel() {
                this.Tables = new FeatureTable[0];
            }

            public HtmlBasicModel FinalModel { get; set; }
            public string LinkTitle { get; set; }
            public string TemplateFileName { get; set; }
            public string OutputFileName { get; set; }
            public IEnumerable<FeatureTable> Tables { get; set; }
        }

        #endregion

        public HtmlOutput(DirectoryInfo templatesDirectory) {
            this.templatesDirectory = templatesDirectory;
        }

        public void Write(DirectoryInfo outputDirectory, IReadOnlyCollection<ResultForAssembly> results, bool keepUpdatingIfTemplatesChange = false) {
            this.RenderAll(outputDirectory, results);
            this.CopyFiles(templatesDirectory, outputDirectory, f => FileExtensionsToCopy.Contains(f.Extension));

            if (keepUpdatingIfTemplatesChange)
                this.KeepUpdatingUsing(outputDirectory, results);
        }

        private void KeepUpdatingUsing(DirectoryInfo outputDirectory, IReadOnlyCollection<ResultForAssembly> results) {
            if (this.watcher == null)
                this.watcher = new FileSystemWatcher(this.templatesDirectory.FullName, "*.cshtml");

            this.watcher.Changed += (sender, e) => {
                if (e.ChangeType != WatcherChangeTypes.Changed)
                    return;

                try {
                    this.RenderAll(outputDirectory, results);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            };
            this.watcher.EnableRaisingEvents = true;
        }

        private void RenderAll(DirectoryInfo outputDirectory, IReadOnlyCollection<ResultForAssembly> results) {
            var customPages = JsonConvert.DeserializeObject<IList<dynamic>>(
                File.ReadAllText(Path.Combine(this.templatesDirectory.FullName, "CustomPages.json"))
            );

            var allModels = Enumerable.Concat(
                customPages.Select(p => new WorkingModel {
                    FinalModel = new HtmlBasicModel { Title = p.Title },
                    LinkTitle = p.Title,
                    TemplateFileName = p.Name + ".cshtml",
                    OutputFileName = p.Name + ".html"
                }),
                results.Select(this.BuildResultModelWithoutNavigation)
            ).ToArray();

            foreach (var model in allModels) {
                this.BuildNavigation(model, allModels);
                this.RenderTemplateWithLayout(model.TemplateFileName, model.FinalModel, outputDirectory, model.OutputFileName);
            }
        }

        private void RenderTemplateWithLayout(string templateFileName, HtmlBasicModel model, DirectoryInfo outputDirectory, string outputFileName) {
            var templateBodyResult = (string)GenericHelper.RewriteAndInvoke(() => this.RenderTemplateToString(templateFileName, (X1)(object)model, null), model.GetType());
            var templateLayoutResult = this.RenderTemplateToString(LayoutTemplateFileName, model, m => m.Body = templateBodyResult);

            var outputPath = Path.Combine(outputDirectory.FullName, outputFileName);
            File.WriteAllText(outputPath, templateLayoutResult);
            ConsoleEx.WriteLine(ConsoleColor.DarkGray, "    rendered " + outputPath);
        }

        private string RenderTemplateToString<TModel>(string templateFileName, TModel model, Action<HtmlTemplateBase<TModel>> initializer = null) {
            initializer = initializer ?? (t => {});

            var templateSource = File.ReadAllText(Path.Combine(this.templatesDirectory.FullName, templateFileName));
            var template = Template.WithBaseType<HtmlTemplateBase<TModel>>((Action<TemplateBase>)(t => initializer((HtmlTemplateBase<TModel>)t)))
                                   .Compile<TModel>(templateSource);

            return template.Render(model);
        }

        private WorkingModel BuildResultModelWithoutNavigation(ResultForAssembly result) {
            var labels = this.GetLabels(result);

            var model = new HtmlResultModel(result.Tables) {
                HtmlAfterAll = this.GetResource(result.Assembly, "AfterAll.html"),
                Title = labels.PageTitle
            }; 
            foreach (var table in result.Tables) {
                model.TableIdMap.Add(table, this.GenerateTableId(table));
            }

            return new WorkingModel {
                FinalModel = model,
                LinkTitle = labels.LinkTitle,
                TemplateFileName = ResultTemplateFileName,
                OutputFileName = result.OutputNamePrefix + ".html",
                Tables = model.Tables
            };
        }

        private void BuildNavigation(WorkingModel currentModel, IReadOnlyCollection<WorkingModel> allModels) {
            foreach (var model in allModels) {
                var url = model.OutputFileName;
                var name = model.LinkTitle;
                var onCurrentPage = model == currentModel;

                var link = new NavigationLinkModel(name, url, onCurrentPage,
                    model.Tables.Select(t => new NavigationLinkModel(
                        t.DisplayName, url + "#" + this.GenerateTableId(t), onCurrentPage
                    )
                ));

                currentModel.FinalModel.Navigation.Add(link);
            }
        }

        private string GenerateTableId(FeatureTable table) {
            var result = table.DisplayName;
            result = Regex.Replace(result, @"\<[^>]+\>", "");
            result = Regex.Replace(result, @"(?<=\W|$)\w", m => m.Value.ToUpperInvariant());
            result = Regex.Replace(result, @"\W+", "");

            return result;
        }

        private dynamic GetLabels(ResultForAssembly result) {
            var labelsString = this.GetResource(result.Assembly, "Labels.json");
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
