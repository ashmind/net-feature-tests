using System;
using System.Collections.Generic;
using System.Linq;
using EmitMapper;
using Omu.ValueInjecter;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class ValueInjecterAdapter : ObjectMapperAdapterBase {
        private readonly ValueInjecter injecter;

        public ValueInjecterAdapter() {
            this.injecter = new ValueInjecter();
        }

        public override Type MapperType {
            get { return typeof(ObjectMapperManager); }
        }

        public override void CreateMap<TSource, TTarget>() {
            // ???
        }

        public override TTarget Map<TTarget>(object source) {
            var target = Activator.CreateInstance<TTarget>();
            this.injecter.Inject(target, source);
            return target;
        }

        public override void Map(object source, object target) {
            this.injecter.Inject(target, source);
        }
    }
}
