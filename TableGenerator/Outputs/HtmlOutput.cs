using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AshMind.Extensions;
using DependencyInjection.TableGenerator.Data;
using DependencyInjection.TableGenerator.Outputs.Html;
using RazorTemplates.Core;

namespace DependencyInjection.TableGenerator.Outputs {
    public class HtmlOutput : IFeatureTableOutput {
        public void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables) {
            // the right way would be to embed it as a resource, but IMHO it is good enough for
            // this type of project
            var templateSource = File.ReadAllText(@"Outputs\Html\FeatureTests.cshtml");
            var template = Template.WithBaseType<HtmlTemplateBase<IEnumerable<FeatureTable>>>()
                                   .Compile<IEnumerable<FeatureTable>>(templateSource);

            var templateResult = template.Render(tables);

            File.WriteAllText(Path.Combine(directory.FullName, "FeatureTests.html"), templateResult);
            File.Copy(@"Outputs\Html\FeatureTests.css", Path.Combine(directory.FullName, "FeatureTests.css"), true);
        }
    }
}
