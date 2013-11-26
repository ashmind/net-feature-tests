using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Mappers;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class AutoMapperAdapter : ObjectMapperAdapterBase {
        private readonly ConfigurationStore configuration;
        private MappingEngine engine;

        public AutoMapperAdapter() {
            this.configuration = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            this.engine = new MappingEngine(this.configuration);
        }

        public override Assembly Assembly {
            get { return typeof(MappingEngine).Assembly; }
        }

        public override void CreateMap<TSource, TTarget>() {
            this.configuration.CreateMap<TSource, TTarget>();
        }

        public override TTarget Map<TTarget>(object source) {
            return this.engine.Map<TTarget>(source);
        }

        public override void Map(object source, object target) {
            this.engine.Map(source, target);
        }
    }
}
