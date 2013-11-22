using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class GenericServiceWithIService2Constraint<T> : IGenericService<T> 
        where T : IService2
    {
    }
}
