using System;
using System.Collections.Generic;
using System.Linq;
using OoMapper;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class OoMapperAdapter : ObjectMapperAdapterBase {
        public override Type MapperType {
            get { return typeof(Mapper); }
        }

        public override void CreateMap<TSource, TTarget>() {
            Mapper.CreateMap<TSource, TTarget>();
        }

        public override TTarget Map<TTarget>(object source) {
            return (TTarget)Mapper.Map(source, source.GetType(), typeof(TTarget));
        }

        public override void Map(object source, object target) {
            Mapper.Map(source, target, source.GetType(), target.GetType());
        }
    }
}
