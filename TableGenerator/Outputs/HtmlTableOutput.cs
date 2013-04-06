using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Outputs {
    public class HtmlTableOutput : IFeatureTableOutput {
        public void Write(IEnumerable<FeatureTable> tables) {
            var html = new StringWriter();
            Action<string> write = html.WriteLine;

            write("<!DOCTYPE html>");
            write("<html>");
            write("  <head>");
            write("    <link rel='stylesheet' href='tables.css' />");
            write("  </head>");
            write("  <body>");
            WriteTables(html, tables);
            write("  </body>");
            write("</html>");

            Directory.CreateDirectory("Outputs");
            File.WriteAllText(@"Outputs\tables.html", html.ToString());
        }

        private void WriteTables(StringWriter writer, IEnumerable<FeatureTable> tables) {
            foreach (var table in tables) {
                writer.WriteLine("    <section>");
                writer.WriteLine("      <h2>{0}</h2>", table.Name);
                writer.WriteLine("      <table>");
                writer.WriteLine("        <tr>");
                writer.WriteLine("          <th>Framework</th>");
                foreach (var header in table.FeatureNames) {
                    writer.WriteLine("          <th>{0}</th>", header);
                }
                writer.WriteLine("        </tr>");

                foreach (var row in table.GetRows()) {
                    writer.WriteLine("        <tr>");
                    writer.WriteLine("          <td>{0}</td>", row.Item1.FrameworkName);
                    foreach (var cell in row.Item2) {
                        WriteCell(writer, cell);
                    }
                    writer.WriteLine("        </tr>");
                }
                writer.WriteLine("      </table>");
                writer.WriteLine("    </section>");
            }
        }

        private static void WriteCell(StringWriter writer, FeatureCell cell) {
            var title = WebUtility.HtmlEncode(cell.Comment ?? "")
                                  .Replace("\n", "&#10;")
                                  .Replace("\r", "&#13;");

            var @class = cell.State.ToString().ToLowerInvariant();

            writer.WriteLine("          <td title='{0}' class='{1}'>{2}</td>", title, @class, WebUtility.HtmlEncode(cell.Text));
        }
    }
}
