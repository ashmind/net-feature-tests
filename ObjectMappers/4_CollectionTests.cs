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
    [DisplayName("Collections (writeable property)")]
    [Description("_TODO_")]
    public class CollectionTests {
        [Feature]
        [DisplayName("Collection ⇒ IEnumerable")]
        public void CollectionToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new Collection<int> { 5 }, Enumerable.Repeat(5, 1));
        }

        [Feature]
        [DisplayName("Collection ⇒ ICollection")]
        public void CollectionToICollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new Collection<int> { 5 }, ((ICollection<int>)new[] { 5 }));
        }
        
        [Feature]
        [DisplayName("List ⇒ IEnumerable")]
        public void ListToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new List<int> { 5 }, Enumerable.Repeat(5, 1));
        }

        [Feature]
        [DisplayName("List ⇒ ICollection")]
        public void ListToCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new List<int> { 5 }, ((ICollection<int>)new[] { 5 }));
        }

        [Feature]
        [DisplayName("List ⇒ IList")]
        public void ListToIList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new List<int> { 5 }, ((IList<int>)new[] { 5 }));
        }

        [Feature]
        [DisplayName("HashSet ⇒ IEnumerable")]
        public void HashSetToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new HashSet<int> { 5 }, Enumerable.Repeat(5, 1));
        }

        [Feature]
        [DisplayName("HashSet ⇒ ISet")]
        public void HashSetToISet(IObjectMapperAdapter mapper) {
            AssertListMappingWorksFor(mapper, new HashSet<int> { 5 }, ((ISet<int>)new HashSet<int> { 5 }));
        }

        private static void AssertListMappingWorksFor<TSourceCollection, TTargetCollection>(IObjectMapperAdapter mapper, TSourceCollection sourceValue, TTargetCollection expectedValue)
            where TSourceCollection : IEnumerable<int>
            where TTargetCollection : IEnumerable<int> 
        {
            mapper.CreateMap<Wrapper<TSourceCollection>, Wrapper<TTargetCollection>>();

            var source = new Wrapper<TSourceCollection> { Value = sourceValue };
            var result = mapper.Map<Wrapper<TTargetCollection>>(source);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedValue.ToArray(), result.Value.ToArray());
        }
    }
}