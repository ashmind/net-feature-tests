using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ObjectWithSingleReadOnlyProperty<TExposed, TActual>
        where TActual : TExposed, new() 
    {
        private readonly TActual value;

        public ObjectWithSingleReadOnlyProperty() {
            this.value = new TActual();
        }

        public TExposed Value {
            get { return this.value; }
        }
    }
}
