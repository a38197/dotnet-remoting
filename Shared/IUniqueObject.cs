using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    public interface IUniqueObject
    {
        string GetUID();
    }

    /**
        Serializable version of IUniqueObject for network propagation. Usefull when proxys may be disposed of.
    **/
    [Serializable]
    public class UniqueObject : IUniqueObject
    {
        public UniqueObject() { } //For serialization
        public UniqueObject(string uid)
        {
            this.uid = uid;
        }

        private readonly string uid;
        public string GetUID()
        {
            return uid;
        }

        public static IUniqueObject copyOf(IUniqueObject original)
        {
            return new UniqueObject(original.GetUID());
        }
    }
}
