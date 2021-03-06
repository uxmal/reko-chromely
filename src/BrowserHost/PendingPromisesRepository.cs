﻿#region
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
using System.Text;

namespace Reko.Chromely.BrowserHost
{
    public class PendingPromisesRepository
    {
        private readonly Dictionary<int, PromiseTask> promises = new Dictionary<int, PromiseTask>();
        private int lastId = 0;
        
        public int AddPromise(PromiseTask promise)
        {
            if(lastId < 0)
            {
                throw new IndexOutOfRangeException(nameof(lastId));
            }

            promises[lastId] = promise;
            return lastId++;
        }

        public PromiseTask RemovePromise(int promiseId)
        {
            var promise = promises[promiseId];
            promises.Remove(promiseId);
            return promise;
        }

        public void Reset()
        {
            promises.Clear();
        }
    }
}
