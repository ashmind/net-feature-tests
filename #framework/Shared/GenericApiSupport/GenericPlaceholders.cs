using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace FeatureTests.Shared.GenericApiSupport.GenericPlaceholders {
// ReSharper restore CheckNamespace

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false)]
    public class GenericPlaceholderAttribute : Attribute {}

    [GenericPlaceholder] public class X1 {}
    [GenericPlaceholder] public class X2 {}

    // ReSharper disable UnusedTypeParameter

    // Those classes provide hierarchy used to match generic inheritance constraints.
    [GenericPlaceholder] public class P<T> {}
    [GenericPlaceholder] public class C<TC, TP> : P<TP> {}

    // ReSharper restore UnusedTypeParameter
}
