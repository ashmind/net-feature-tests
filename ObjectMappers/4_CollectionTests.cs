using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AshMind.Extensions;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(4)]
    [DisplayName("Collections (writeable property)")]
    [Description("_TODO_")]
    public class CollectionTests {
        [Feature]
        [DisplayName("ICollection ⇒ IEnumerable")]
        public void CollectionToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IEnumerable ⇒ ICollection")]
        public void EnumerableToCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }
        
        [Feature]
        [DisplayName("ICollection ⇒ IReadOnlyCollection")]
        public void CollectionToReadOnlyCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IReadOnlyCollection ⇒ ICollection")]
        public void ReadOnlyCollectionToCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IReadOnlyCollection ⇒ IEnumerable")]
        public void ReadOnlyCollectionToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }
        
        [Feature]
        [DisplayName("IList ⇒ IEnumerable")]
        public void ListToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IEnumerable ⇒ IList")]
        public void EnumerableToList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IList ⇒ ICollection")]
        public void ListToCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("ICollection ⇒ IList")]
        public void CollectionToList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IList ⇒ IReadOnlyCollection")]
        public void ListToReadOnlyCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IReadOnlyCollection ⇒ IList")]
        public void ReadOnlyCollectionToList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }
        
        [Feature]
        [DisplayName("IList ⇒ IReadOnlyList")]
        public void ListToReadOnlyList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IReadOnlyList ⇒ IList")]
        public void ReadOnlyListToList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IEnumerable ⇒ IReadOnlyList")]
        public void EnumerableToReadOnlyList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("ISet ⇒ IEnumerable")]
        public void SetToEnumerable(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IEnumerable ⇒ ISet")]
        public void EnumerableToSet(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("ISet ⇒ ICollection")]
        public void SetToCollection(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }
        
        [Feature]
        [DisplayName("ICollection ⇒ ISet")]
        public void CollectionToSet(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("ISet ⇒ IList")]
        public void SetToList(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        [Feature]
        [DisplayName("IList ⇒ ISet")]
        public void ListToSet(IObjectMapperAdapter mapper) {
            AssertListMappingWorksBasedOnTestName(mapper);
        }

        private static void AssertListMappingWorksBasedOnTestName(IObjectMapperAdapter mapper, [CallerMemberName] string name = null) {
            var parts = name.Split("To");
            var sourceType = GetCollectionType(parts[0]);
            var targetType = GetCollectionType(parts[1]);

            GenericHelper.RewriteAndInvoke(() => AssertListMappingWorksFor<IX1, IX2>(mapper), sourceType, targetType);
        }

        private static Type GetCollectionType(string namePart) {
            var fullName = "System.Collections.Generic.I" + namePart + "`1";
            var openType = new[] { typeof(IList<>), typeof(ISet<>) }
                .Select(t => t.Assembly)
                .Distinct()
                .Select(a => a.GetType(fullName))
                .SingleOrDefault(t => t != null);

            if (openType == null)
                throw new InvalidOperationException("Could not find type '" + fullName + "'.");

            return openType.MakeGenericType(typeof(int));
        }

        private static void AssertListMappingWorksFor<TSourceCollection, TTargetCollection>(IObjectMapperAdapter mapper)
            where TSourceCollection : IEnumerable<int>
            where TTargetCollection : IEnumerable<int>
        {
            var sourceValue = new[] { 5 }.AsEnumerable();
            if (typeof(TSourceCollection) == typeof(ISet<int>))
                sourceValue = new HashSet<int> { 5 };

            mapper.CreateMap<Wrapper<TSourceCollection>, Wrapper<TTargetCollection>>();

            var source = new Wrapper<TSourceCollection> { Value = (TSourceCollection)sourceValue };
            var result = mapper.Map<Wrapper<TTargetCollection>>(source);
            Assert.NotNull(result.Value);
            Assert.Equal(sourceValue.ToArray(), result.Value.ToArray());
        }

        [GenericPlaceholder] private interface IX1 : IEnumerable<int> {}
        [GenericPlaceholder] private interface IX2 : IEnumerable<int> {}
    }
}