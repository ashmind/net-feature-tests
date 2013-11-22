using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace ObjectMappers.FeatureTests.Adapters {
    public abstract class ObjectMapperAdapterBase : LibraryAdapterBase, IObjectMapperAdapter {
        public abstract TTarget Map<TTarget>(object source);
        public abstract void Map(object source, object target);
    }
}
