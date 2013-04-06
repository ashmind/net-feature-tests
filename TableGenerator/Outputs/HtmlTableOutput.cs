using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Outputs {
    public class HtmlTableOutput : IFeatureTableOutput {
        public void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables) {
            var builder = new StringBuilder();

            builder.AppendLine("<!DOCTYPE html>")
                   .AppendLine("<html>")
                   .AppendLine("  <head>")
                   .AppendLine("    <link rel='stylesheet' href='FeatureTests.css' />")
                   .AppendLine("  </head>")
                   .AppendLine("  <body>")
                   .AppendLine("  <body>");
            
            AppendTables(builder, tables);
            builder.AppendLine("  </body>")
                   .AppendLine("</html>");

            File.WriteAllText(Path.Combine(directory.FullName, "FeatureTests.html"), builder.ToString());

            // obviously the right way would be to embed it as resource, but IMHO it is good enough for
            // this type of project
            File.Copy(@"Outputs\FeatureTests.css", Path.Combine(directory.FullName, "FeatureTests.css"), true);
        }

        private void AppendTables(StringBuilder builder, IEnumerable<FeatureTable> tables) {
            foreach (var table in tables) {
                builder.AppendLine("    <section>")
                       .AppendFormat("      <h2>{0}</h2>", table.Name).AppendLine()
                       .AppendLine("      <table>")
                       .AppendLine("        <tr>")
                       .AppendLine("          <th>Framework</th>");
                foreach (var header in table.FeatureNames) {
                    builder.AppendFormat("          <th>{0}</th>", header).AppendLine();
                }
                builder.AppendLine("        </tr>");

                foreach (var row in table.GetRows()) {
                    builder.AppendLine("        <tr>")
                           .AppendFormat("          <td>{0}</td>", row.Item1.FrameworkName).AppendLine();
                    foreach (var cell in row.Item2) {
                        AppendCell(builder, cell);
                    }
                    builder.AppendLine("        </tr>");
                }
                builder.AppendLine("      </table>")
                       .AppendLine("    </section>");
            }
        }

        private static void AppendCell(StringBuilder builder, FeatureCell cell) {
            var title = WebUtility.HtmlEncode(cell.Comment ?? "")
                                  .Replace("\n", "&#10;")
                                  .Replace("\r", "&#13;");

            var @class = cell.State.ToString().ToLowerInvariant();

            builder.AppendFormat("          <td title='{0}' class='{1}'>{2}</td>", title, @class, WebUtility.HtmlEncode(cell.Text))
                   .AppendLine();
        }
    }
}
