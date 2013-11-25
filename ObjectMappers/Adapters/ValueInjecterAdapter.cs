using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using Omu.ValueInjecter;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class ValueInjecterAdapter : ObjectMapperAdapterBase {
        private readonly ValueInjecter injecter;

        public ValueInjecterAdapter() {
            this.injecter = new ValueInjecter();
        }

        public override Assembly Assembly {
            get { return typeof(ObjectMapperManager).Assembly; }
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
