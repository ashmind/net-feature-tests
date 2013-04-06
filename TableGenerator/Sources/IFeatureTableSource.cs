using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Sources {
    public interface IFeatureTableSource {
        IEnumerable<FeatureTable> GetTables();
    }
}
