using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    public interface IStockManager : IUniqueObject
    {
        string StockManagerName { get; }
        bool HasProduct(string productName);
        bool HasProductFromFamily(string familyName);
        ProductBase GetProduct(string name);
        ProductBase[] GetProducts(string family);
        string[] GetProductFamilies();
    }

    [Serializable]
    public abstract class ProductBase
    {
        public virtual string Name { get; set; }
        public virtual string Family { get; set; }
        public virtual int Stock { get; set; }
        public virtual decimal Price { get; set; }
    }
}
