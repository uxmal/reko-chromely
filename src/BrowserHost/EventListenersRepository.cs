using System;
using System.Collections.Generic;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class EventHandlerMap
    {
        private readonly Dictionary<int, CefV8Value> eventHandlers = new Dictionary<int, CefV8Value>();
        private int lastId = 0;

        public int AddEventListener(CefV8Value handler)
        {
            if (lastId < 0)
            {
                throw new IndexOutOfRangeException(nameof(lastId));
            }

            eventHandlers[lastId] = handler;
            return lastId++;
        }

        public void RemoveEventListener(int listenerId)
        {
            eventHandlers.Remove(listenerId);
        }

        public void Invoke(CefV8Value[] arguments)
        {
            foreach (var handler in eventHandlers.Values)
            {
                handler.ExecuteFunction(null, arguments);
            }
        }
    }

    public class EventListenersRepository
    {

        private readonly Dictionary<string, EventHandlerMap> eventListeners = new Dictionary<string, EventHandlerMap>();

        public void RegisterEvent(string eventName)
        {
            eventListeners[eventName] = new EventHandlerMap();
        }

        public int AddEventListener(string eventName, CefV8Value handler)
        {
            return eventListeners[eventName].AddEventListener(handler);
        }

        public void RemoveEventListener(string eventName, int listenerId)
        {
            eventListeners[eventName].RemoveEventListener(listenerId);
        }

        public void Invoke(string eventName, CefV8Value[] arguments)
        {
            eventListeners[eventName].Invoke(arguments);
        }

        public void Reset()
        {
            eventListeners.Clear();
        }
    }
}
