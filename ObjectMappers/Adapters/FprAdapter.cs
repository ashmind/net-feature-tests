using System;
using System.Collections.Generic;
using System.Linq;
using Fpr;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class FprAdapter : ObjectMapperAdapterBase {
        public override Type MapperType {
            get { return typeof(TypeAdapter); }
        }

        public override void CreateMap(Type sourceType, Type targetType) {
        }

        public override TTarget Map<TTarget>(object source) {
            return TypeAdapter.Adapt<TTarget>(source);
        }

        public override void Map(object source, object target) {
            TypeAdapter.Adapt(source, target);
        }
    }
}
