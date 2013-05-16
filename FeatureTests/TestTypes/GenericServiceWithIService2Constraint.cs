using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class GenericServiceWithIService2Constraint<T> : IGenericService<T> 
        where T : IService2
    {
    }
}
