using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public static class ObjectMapperAdapterExtensions {
        public static void CreateMap<TSource, TTarget>(this IObjectMapperAdapter mapper) {
            mapper.CreateMap(typeof(TSource), typeof(TTarget));
        }
    }
}
