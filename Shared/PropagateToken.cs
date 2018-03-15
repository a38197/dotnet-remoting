using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    [Serializable]
    public enum PropagateAction
    {
        Register, Unregister, AddServer, RemoveServer, GetStockManager
    }

    [Serializable]
    public class PropagateToken : IUniqueObject
    {
        public object Data { get; set; }
        public PropagateAction Action { get; set; }
        public PropagateToken(PropagateAction action)
        {
            Action = action;
            hops = new string[0];
        }
        public PropagateToken(PropagateAction action, object data) : this(action)
        {
            Data = data;
        }

        private string uid = Guid.NewGuid().ToString();
        public string GetUID()
        {
            return uid;
        }

        private string[] hops;
        public bool HasHopedOnObject(IUniqueObject uid)
        {
            return hops.Contains(uid.GetUID());
        }

        public PropagateToken AddHop(IUniqueObject uid)
        {
            if (HasHopedOnObject(uid))
                return this;

            string[] newHops = new string[hops.Length+1];
            Array.Copy(hops, newHops, hops.Length);
            newHops[hops.Length] = uid.GetUID();
            hops = newHops;
            return this;
        }
    }
}
