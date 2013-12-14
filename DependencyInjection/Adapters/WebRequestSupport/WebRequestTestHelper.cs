using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AshMind.Extensions;
using ReflectionMagic;

namespace FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport {
    public class WebRequestTestHelper : IDisposable {
        private readonly ISet<IHttpModule> modules = new HashSet<IHttpModule>();
        private readonly TestHttpApplication application;

        //public static WebRequestTestHelper Current {
        //    get { return (WebRequestTestHelper)CallContext.LogicalGetData("WebRequestTestHelper.Current"); }
        //    private set { CallContext.LogicalSetData("WebRequestTestHelper.Current", value); }
        //}

        //public WebRequestTestHelper() {
        //    if (Current != null)
        //        throw 
        //}

        public WebRequestTestHelper() {
            this.application = new TestHttpApplication();
        }

        public void RegisterModule<THttpModule>()
            where THttpModule : IHttpModule, new()
        {
            RegisterModule(new THttpModule());
        }

        public void RegisterModule(IHttpModule module) {
            module.Init(this.application);
            this.modules.Add(module);
        }

        public void BeginWebRequest() {
            var context = new HttpContext(
                new HttpRequest("", "http://only.test", ""),
                new HttpResponse(new StringWriter())
            ) { ApplicationInstance = this.application };
            this.application.AsDynamic()._context = context;

            HttpContext.Current = context;

            this.application.RaiseEvent("BeginRequest");
        }

        public void EndWebRequest() {
            this.application.RaiseEvent("EndRequest");
            HttpContext.Current = null;
        }
        
        public void Dispose() {
            HttpContext.Current = null;
            modules.ForEach(m => m.Dispose());
            modules.Clear();
        }
    }
}
