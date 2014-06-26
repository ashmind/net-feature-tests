using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using ThisMember.Core;

namespace FeatureTests.On.ObjectMappers.Adapters {
    public class ThisMemberAdapter : ObjectMapperAdapterBase {
        private readonly MemberMapper mapper;

        public ThisMemberAdapter() {
            this.mapper = new MemberMapper();
        }

        public override Type MapperType {
            get { return typeof(MemberMapper); }
        }

        public override void CreateMap(Type sourceType, Type targetType) {
            this.mapper.CreateMap(sourceType, targetType);
        }

        public override TTarget Map<TTarget>(object source) {
            return (TTarget)GenericHelper.RewriteAndInvoke(() => this.mapper.Map<X1>(source), typeof(TTarget));
        }

        public override void Map(object source, object target) {
            this.mapper.Map(source, target);
        }
    }
}
