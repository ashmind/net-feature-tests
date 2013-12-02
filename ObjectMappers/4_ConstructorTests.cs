using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(4)]
    [DisplayName("Constructor parameters")]
    public class ConstructorParameterTests {
        [Feature]
        [DisplayName("Simple")]
        public void Simple(IObjectMapperAdapter mapper) {
            mapper.CreateMap<Wrapper<string>, ImmutableWrapper<string>>();

            var source = new Wrapper<string> { Value = "X" };
            var result = mapper.Map<ImmutableWrapper<string>>(source);

            Assert.Equal("X", result.Value);
        }

        [Feature]
        [DisplayName("Nested")]
        public void Nested(IObjectMapperAdapter mapper) {
            mapper.CreateMap<Wrapper<string>, ImmutableWrapper<string>>();
            mapper.CreateMap<Wrapper<Wrapper<string>>, ImmutableWrapper<ImmutableWrapper<string>>>();

            var source = new Wrapper<Wrapper<string>> { Value = new Wrapper<string> { Value = "X" }};
            var result = mapper.Map<ImmutableWrapper<ImmutableWrapper<string>>>(source);

            Assert.Equal("X", result.Value.Value);
        }
    }
}