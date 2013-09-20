using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace DependencyInjection.FeatureTests.Adapters.Support.GenericPlaceholders {
// ReSharper restore CheckNamespace

    public class X1 {}
    public class X2 {}

    // ReSharper disable UnusedTypeParameter

    // Those classes provide hierarchy used to match generic inheritance constraints.
    public class P<T> {}
    public class C<TC, TP> : P<TP> {}

    // ReSharper restore UnusedTypeParameter
}
