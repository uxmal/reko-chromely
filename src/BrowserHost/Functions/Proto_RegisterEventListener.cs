using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost.Functions
{
    public class Proto_RegisterEventListener : CefV8Handler
    {
        private readonly EventListenersRepository eventListeners;

        public Proto_RegisterEventListener(EventListenersRepository eventListeners)
        {
            this.eventListeners = eventListeners;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var eventName = arguments[0].GetStringValue();
            var eventHandler = arguments[1];

            int listenerId = eventListeners.AddEventListener(eventName, eventHandler);

            exception = null!;
            returnValue = CefV8Value.CreateInt(listenerId);
            return true;
        }
    }
}
