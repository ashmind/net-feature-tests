using System;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class DisposableService : IDisposable {
        public void Dispose() {
            this.Disposed = true;
        }

        public bool Disposed { get; private set; }
    }
}