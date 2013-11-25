using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public abstract class ObjectMapperAdapterBase : LibraryAdapterBase, IObjectMapperAdapter {
        public abstract void CreateMap<TSource, TTarget>();
        public abstract TTarget Map<TTarget>(object source);
        public abstract void Map(object source, object target);
    }
}
