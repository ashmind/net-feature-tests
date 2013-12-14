using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport {
    public class TestHttpApplication : HttpApplication {
        public void RaiseEvent(string name) {
            var handler = (EventHandler)this.Events[GetEventKey(name)];
            if (handler == null)
                return;

            handler(this, EventArgs.Empty);
        }

        private object GetEventKey(string name) {
            var keyField = typeof(HttpApplication).GetField("Event" + name, BindingFlags.Static | BindingFlags.NonPublic);
            return keyField.GetValue(null);
        }
    }
}
