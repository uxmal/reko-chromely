using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class PromiseTask : CefTask
    {
        private readonly CefV8Context ctx;

        private readonly Action<PromiseTask> func;
        private readonly CefV8Value resolveCb;
        private readonly CefV8Value rejectCb;

        public PromiseTask(
            CefV8Context ctx,
            Action<PromiseTask> func,
            CefV8Value resolveCb, CefV8Value rejectCb
        )
        {
            this.ctx = ctx;
            this.func = func;
            this.resolveCb = resolveCb;
            this.rejectCb = rejectCb;
        }

        public void Resolve(CefV8Value result)
        {
            this.resolveCb.ExecuteFunctionWithContext(ctx, null, new CefV8Value[] { result });
        }

        public void Reject(CefV8Value reason)
        {
            this.rejectCb.ExecuteFunctionWithContext(ctx, null, new CefV8Value[] { reason });
        }

        protected override void Execute()
        {
            func.Invoke(this);
        }
    }
}
