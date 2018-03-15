using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSoftware.Shared;

namespace SuperSoftware.Client
{
    public class CachedStockManager
    {
        public IStockManager Manager { get; set; }
        public string[] Families { get; set; }
        public string UID { get; set; }

        public CachedStockManager(IStockManager manager)
        {
            Manager = manager;
            Families = manager.GetProductFamilies();
            UID = manager.GetUID();
        }
    }
}
