using System;
using System.Collections.Generic;
using System.Text;
using DependencyInjection.FeatureTests.Adapters;

namespace DependencyInjection.FeatureTests {
    public static class Frameworks {
        public static IEnumerable<IFrameworkAdapter> List() {
            yield return new AutofacAdapter();
            yield return new CastleAdapter();
            yield return new LinFuAdapter();
            yield return new NinjectAdapter();
            yield return new StructureMapAdapter();
            yield return new SpringAdapter();
            yield return new UnityAdapter();
        }
    }
}
