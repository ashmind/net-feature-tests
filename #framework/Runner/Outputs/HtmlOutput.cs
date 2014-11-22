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
using HtmlAgilityPack;
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

        #region Intermediate

        private class Section {
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }

        private class IntermediateModel {
            public IntermediateModel() {
                this.Sections = new List<Section>();
            }

            public HtmlBasicModel FinalModel { get; set; }
            public string LinkTitle { get; set; }
            public string TemplateFileName { get; set; }
            public string OutputFileName { get; set; }
            public IList<Section> Sections { get; private set; }
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
                customPages.Select(p => new IntermediateModel {
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
            FluentConsole.DarkGray.Line("    rendered " + outputPath);
        }

        private string RenderTemplateToString<TModel>(string templateFileName, TModel model, Action<HtmlTemplateBase<TModel>> initializer = null) {
            initializer = initializer ?? (t => {});

            var templateSource = File.ReadAllText(Path.Combine(this.templatesDirectory.FullName, templateFileName));
            var template = Template.WithBaseType<HtmlTemplateBase<TModel>>((Action<TemplateBase>)(t => initializer((HtmlTemplateBase<TModel>)t)))
                                   .Compile<TModel>(templateSource);

            return template.Render(model);
        }

        private IntermediateModel BuildResultModelWithoutNavigation(ResultForAssembly result) {
            var options = this.GetOptions(result);

            var model = new HtmlResultModel(result.Tables) {
                Title = options.PageTitle,
                HtmlBeforeAll = this.GetResource(result.Assembly, "BeforeAll.html"),
                HtmlAfterAll = this.GetResource(result.Assembly, "AfterAll.html"),
                TotalVisible = !((bool?)options.HideTotal ?? false)
            };
            var intermediate = new IntermediateModel {
                FinalModel = model,
                LinkTitle = options.LinkTitle,
                TemplateFileName = ResultTemplateFileName,
                OutputFileName = result.OutputNamePrefix + ".html"
            };

            AddSectionsFromHtml(intermediate, model.HtmlBeforeAll);
            foreach (var table in result.Tables) {
                var id = this.GenerateTableId(table);
                model.TableIdMap.Add(table, id);
                intermediate.Sections.Add(new Section { Id = id, DisplayName = table.DisplayName });
            }
            AddSectionsFromHtml(intermediate, model.HtmlAfterAll);

            return intermediate;
        }

        private void AddSectionsFromHtml(IntermediateModel intermediate, string html) {
            var document = new HtmlDocument();
            document.LoadHtml("<html>" + html + "</html>");

            var headerRegex = new Regex(@"h\d", RegexOptions.IgnoreCase);
            var sections = document.DocumentNode.Descendants()
                                                .Where(n => headerRegex.IsMatch(n.Name))
                                                .Where(h => h.Attributes.Contains("id"))
                                                .Select(h => new Section { Id = h.GetAttributeValue("id", ""), DisplayName = h.InnerText });

            intermediate.Sections.AddRange(sections);
        }

        private void BuildNavigation(IntermediateModel currentModel, IReadOnlyCollection<IntermediateModel> allModels) {
            foreach (var model in allModels) {
                var url = model.OutputFileName;
                var name = model.LinkTitle;
                var onCurrentPage = model == currentModel;

                var link = new NavigationLinkModel(name, url, onCurrentPage,
                    model.Sections.Select(s => new NavigationLinkModel(
                        s.DisplayName, url + "#" + s.Id, onCurrentPage
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

        private dynamic GetOptions(ResultForAssembly result) {
            var optionsString = this.GetResource(result.Assembly, "Options.json");
            return JsonConvert.DeserializeObject(optionsString.NullIfEmpty() ?? "{}");
        }

        private string GetResource(Assembly assembly, string name) {
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".Html." + name));
            if (resourceName == null)
                return "";

            FluentConsole.DarkGray.Line("    found " + resourceName);

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

                FluentConsole.DarkGray.Line("    hardlinked " + targetPath);
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
