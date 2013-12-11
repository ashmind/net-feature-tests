using System;
using System.Collections.Generic;
using System.Linq;
using EmitMapper;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class EmitMapperAdapter : ObjectMapperAdapterBase {
        private readonly ObjectMapperManager manager;

        public EmitMapperAdapter() {
            this.manager = new EmitMapper.ObjectMapperManager();
        }

        public override Type MapperType {
            get { return typeof(ObjectMapperManager); }
        }

        public override void CreateMap<TSource, TTarget>() {
            // ???
        }

        public override TTarget Map<TTarget>(object source) {
            return (TTarget)GenericHelper.RewriteAndInvoke(
                () => this.manager.GetMapper<X1, TTarget>().Map((X1)source),
                source.GetType()
            );
        }

        public override void Map(object source, object target) {
            GenericHelper.RewriteAndInvoke(
                () => this.manager.GetMapper<X1, X2>().Map((X1)source, (X2)target),
                source.GetType(), target.GetType()
            );
        }
    }
}
