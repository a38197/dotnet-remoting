using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Server
{
    [Serializable]
    public abstract class ServerException : Shared.SuperSoftwareException
    {
        public ServerException() : base() { }
        public ServerException(string msg) : base(msg) { }
        public ServerException(string msg, Exception ex) : base(msg,ex) { }
    }

    [Serializable]
    public class DiscoveryException : ServerException
    {
        public DiscoveryException() : base() { }
        public DiscoveryException(string msg) : base(msg) { }
        public DiscoveryException(string msg, Exception ex) : base(msg,ex) { }
    }
}
