using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Mappers;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class AutoMapperAdapter : ObjectMapperAdapterBase {
        private readonly ConfigurationStore configuration;
        private readonly MappingEngine engine;

        public AutoMapperAdapter() {
            this.configuration = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            this.engine = new MappingEngine(this.configuration);
        }

        public override Type MapperType {
            get { return typeof(MappingEngine); }
        }

        public override void CreateMap(Type sourceType, Type targetType) {
            this.configuration.CreateMap(sourceType, targetType);
        }

        public override TTarget Map<TTarget>(object source) {
            return this.engine.Map<TTarget>(source);
        }

        public override void Map(object source, object target) {
            this.engine.Map(source, target);
        }
    }
}
