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
    [DisplayOrder(5)]
    [DisplayName("Collections (read-only property)")]
    [Description(@"
        Having collections as read-only property is a basic .NET design guideline (even object initializers support it).  
        It is easy to disregard for DTOs. However, the mapper can be used for mapping to proper domain objects as well.
    ")]
    public class CollectionAsReadOnlyPropertyTests {
        [Feature]
        [DisplayName("IList ⇒ IList")]
        public void IListToIList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor<IList<int>, IList<int>, List<int>>(mapper, new List<int> { 5 });
        }

        [Feature]
        [DisplayName("ICollection ⇒ ICollection")]
        public void ICollectionToICollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor<ICollection<int>, ICollection<int>, Collection<int>>(mapper, new Collection<int> { 5 });
        }

        [Feature]
        [DisplayName("ISet ⇒ ISet")]
        public void ISetToISet(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor<ISet<int>, ISet<int>, HashSet<int>>(mapper, new HashSet<int> { 5 });
        }

        private static void AssertListMappingWorksFor<TSourceCollection, TExposedTargetCollection, TActualTargetCollection>(IObjectMapperAdapter mapper, TSourceCollection sourceValue)
            where TSourceCollection : IEnumerable<int>
            where TExposedTargetCollection : IEnumerable<int>
            where TActualTargetCollection : TExposedTargetCollection, new() 
        {
            mapper.CreateMap<Wrapper<TSourceCollection>, WrapperWithReadOnly<TExposedTargetCollection, TActualTargetCollection>>();

            var source = new Wrapper<TSourceCollection> { Value = sourceValue };
            var result = mapper.Map<WrapperWithReadOnly<TExposedTargetCollection, TActualTargetCollection>>(source);

            Assert.NotNull(result.Value);
            Assert.Equal(sourceValue.ToArray(), result.Value.ToArray());
        }
    }
}