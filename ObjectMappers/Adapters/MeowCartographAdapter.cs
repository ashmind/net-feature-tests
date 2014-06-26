using System;
using System.Collections.Generic;
using System.Linq;
using Meow.Cartograph;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class MeowCartographAdapter : ObjectMapperAdapterBase {
        public override string Name {
            get { return PackageId; }
        }

        public override Type MapperType {
            get { return typeof(Mapper); }
        }

        public override void CreateMap(Type sourceType, Type targetType) {
        }

        public override TTarget Map<TTarget>(object source) {
            return source.MapAs<TTarget>();
        }

        public override void Map(object source, object target) {
            // !!!
        }
    }
}
