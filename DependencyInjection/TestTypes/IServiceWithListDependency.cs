using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public interface IServiceWithListDependency<out TServiceList> 
        where TServiceList : IEnumerable<IService>
    {
        TServiceList Services { get; }
    }
}
