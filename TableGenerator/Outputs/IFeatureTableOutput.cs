using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Outputs {
    public interface IFeatureTableOutput {
        void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables);
    }
}
