using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.Shared;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(1)]
    [DisplayName("Essential functionality")]
    [Description("These are the basic features that should be supported by all mappers.")]
    public class BasicTests {
        [Feature]
        [DisplayName("I am test")]
        public void IAmTest(IObjectMapperAdapter mapper) {
            
        }
    }
}