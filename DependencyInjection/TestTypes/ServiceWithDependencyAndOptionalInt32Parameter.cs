using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithDependencyAndOptionalInt32Parameter {
        public int Optional { get; private set; }

        public ServiceWithDependencyAndOptionalInt32Parameter(IService service, int optional = 5) {
            Optional = optional;
        }
    }
}
