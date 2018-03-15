using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;

namespace SuperSoftware.Shared
{
    public static class Utils
    {
        public static System.Runtime.Remoting.Proxies.RealProxy GetRealProxy(object proxy)
        {
            if (!RemotingServices.IsTransparentProxy(proxy))
                throw new ArgumentException("Not a tranparent proxy");

            return RemotingServices.GetRealProxy(proxy);
        }
    }
}
