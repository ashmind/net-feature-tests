using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using Pelusoft.EasyMapper;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class EasyMapperAdapter : ObjectMapperAdapterBase {
        public override Type MapperType {
            get { return typeof(EasyMapper); }
        }

        public override string PackageId {
            get { return "EasyMapper"; }
        }

        public override void CreateMap<TSource, TTarget>() {
            // ???
        }

        public override TTarget Map<TTarget>(object source) {
            return (TTarget)GenericHelper.RewriteAndInvoke(
                () => EasyMapper.Map<X1>(source), typeof(TTarget)
            );
        }

        public override void Map(object source, object target) {
            // ???
        }
    }
}
