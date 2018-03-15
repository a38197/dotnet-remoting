using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    public class SuperAsyncResult<T> : IAsyncResult
    {

        public SuperAsyncResult(object state)
        {
            asyncState = state;
        }

        private object asyncState;
        public object AsyncState
        {
            get
            {
                return asyncState;
            }
        }

        private ManualResetEvent handle = new ManualResetEvent(false);
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return handle;
            }
        }

        public bool completedSynchronously;
        public bool CompletedSynchronously
        {
            get
            {
                return completedSynchronously;
            }
        }

        private volatile bool isCompleted;
        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
        }

        private T value = default(T);
        private Exception ex = null;
        public T GetResult()
        {
            lock (value)
            {
                for (;;)
                {
                    if (!isCompleted)
                        handle.WaitOne();
                    else
                        break;
                }
                if (ex != null)
                    throw ex;

                return value;
            }
        }

        public void SetComplete(T val)
        {
            value = val;
            isCompleted = true;
            handle.Set();
        }

        public void SetError(Exception e)
        {
            ex = e;
            isCompleted = true;
            handle.Set();
        }
        
    }
}
