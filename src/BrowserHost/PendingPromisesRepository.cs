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

        public PromiseTask PopPromise(int promiseId)
        {
            var promise = promises[promiseId];
            promises.Remove(promiseId);
            return promise;
        }
    }
}
