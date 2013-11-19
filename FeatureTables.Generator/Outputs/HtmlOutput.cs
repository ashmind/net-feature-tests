using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.Outputs.Html;
using RazorTemplates.Core;

namespace DependencyInjection.FeatureTables.Generator.Outputs {
    public class HtmlOutput : IFeatureTableOutput {
        public void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables) {
            // the right way would be to embed it as a resource, but IMO it is good enough for
            // this type of project
            var templateSource = File.ReadAllText(@"Outputs\Html\FeatureTests.cshtml");
            var template = Template.WithBaseType<HtmlTemplateBase<IEnumerable<FeatureTable>>>()
                                   .Compile<IEnumerable<FeatureTable>>(templateSource);

            var templateResult = template.Render(tables);

            File.WriteAllText(Path.Combine(directory.FullName, "FeatureTests.html"), templateResult);
            File.Copy(@"Outputs\Html\FeatureTests.css", Path.Combine(directory.FullName, "FeatureTests.css"), true);

            var targetJsDirectory = new DirectoryInfo(Path.Combine(directory.FullName, "js"));
            if (!targetJsDirectory.Exists)
                targetJsDirectory.Create();
            foreach (var file in new DirectoryInfo(@"Outputs\Html\js").GetFiles()) {
                file.CopyTo(Path.Combine(targetJsDirectory.FullName, file.Name), true);    
            }
        }
    }
}
