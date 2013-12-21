using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public interface IObjectMapperAdapter : ILibrary {
        Type MapperType { get; }
        void CreateMap(Type sourceType, Type targetType);

        TTarget Map<TTarget>(object source);
        void Map(object source, object target);
    }
}
