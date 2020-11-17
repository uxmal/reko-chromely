#region License
// Copyright 2020 the Reko contributors.

// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the
// 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
#endregion

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
