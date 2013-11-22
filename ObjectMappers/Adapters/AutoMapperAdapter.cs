using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

namespace ObjectMappers.FeatureTests.Adapters {
    public class AutoMapperAdapter : ObjectMapperAdapterBase {
        private readonly ConfigurationStore configuration;
        private readonly MappingEngine engine;

        public AutoMapperAdapter() {
            this.configuration = new ConfigurationStore(new TypeMapFactory(), Enumerable.Empty<IObjectMapper>());
            this.engine = new MappingEngine(this.configuration);
        }

        public override Assembly Assembly {
            get { return typeof(MappingEngine).Assembly; }
        }

        public override TTarget Map<TTarget>(object source) {
            return this.engine.Map<TTarget>(source);
        }

        public override void Map(object source, object target) {
            this.engine.Map(source, target);
        }
    }
}
