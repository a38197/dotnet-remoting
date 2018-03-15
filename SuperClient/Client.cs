using System;
using System.Collections.Generic;
using System.Linq;
using SuperSoftware.Shared;
using System.Threading;
using System.Runtime.Remoting;

namespace SuperSoftware.Client
{
    [Serializable]
    public class Client : MarshalByRefObject, IClient
    {


        private readonly StockManager stockManager;
        private readonly string name;

        public Client(string name) {
            this.name = name;
            stockManager = new StockManager(name);
        }


        private string uid = Guid.NewGuid().ToString();
        public string GetUID()
        {
            return uid;
        }

        public IStockManager StockManager
        {
            get
            {
                return stockManager;
            }
        }
        
        public void RegisterRequestEvent(StockManager.RequestInfo evt)
        {
            stockManager.OnRequestInfo += evt;
        }

        public void SetProducts(Product[] stock)
        {
            stockManager.SetProductList(stock);
        }
    }
}
