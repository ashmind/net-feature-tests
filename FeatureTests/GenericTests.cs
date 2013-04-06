using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    public class GenericTests {
        [ForEachFramework]
        public void OpenGenericTypes(IFrameworkAdapter framework) {
            framework.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = framework.GetInstance<IGenericService<int>>();

            Assert.NotNull(resolved);
        }
    }
}
