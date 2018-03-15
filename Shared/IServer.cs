using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    public interface IServer : IUniqueObject
    {
        void Register(string[] productFamilies, IStockManager sm);
        void Unregister(IStockManager sm);
        void SendMessage(string msg);
        IStockManager[] GetStockManagers(string productFamily, IUniqueObject sender);
    }
}
