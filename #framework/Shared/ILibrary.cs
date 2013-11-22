using System.Reflection;

namespace FeatureTests.Shared {
    public interface ILibrary {
        string Name        { get; }
        Assembly Assembly  { get; }
        string PackageId   { get; }
    }
}