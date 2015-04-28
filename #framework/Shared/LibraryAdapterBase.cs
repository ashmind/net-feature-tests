using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace FeatureTests.Shared {
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public abstract class LibraryAdapterBase : ILibrary {
        public virtual string Name {
            get { return Regex.Match(GetType().Name, "^(.+?)(?:Adapter)?$").Groups[1].Value; }
        }

        [CanBeNull]
        public abstract Assembly Assembly { get; }

        public virtual string PackageId {
            get { return Assembly.GetName().Name; }
        }
    }
}
