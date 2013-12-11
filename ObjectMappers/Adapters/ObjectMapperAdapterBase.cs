using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.Shared;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public abstract class ObjectMapperAdapterBase : LibraryAdapterBase, IObjectMapperAdapter {
        public abstract Type MapperType { get; }
        public abstract void CreateMap<TSource, TTarget>();
        public abstract TTarget Map<TTarget>(object source);
        public abstract void Map(object source, object target);

        public override Assembly Assembly {
            get { return MapperType.Assembly; }
        }
    }
}
