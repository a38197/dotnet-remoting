using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    public interface IClient : IUniqueObject
    {
        IStockManager StockManager { get; }
    }
}
