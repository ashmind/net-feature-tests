using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public interface IObjectMapperAdapter : ILibrary {
        TTarget Map<TTarget>(object source);
        void Map(object source, object target);
    }
}
