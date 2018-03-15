using System;
using System.Collections.Generic;
using System.Linq;
using SuperSoftware.Shared;

namespace SuperSoftware.Client
{
    public class StockManager : MarshalByRefObject, IStockManager
    {

        public StockManager(string name)
        {
            this.name = name;
        }

        private Dictionary<string, Product> productList = new Dictionary<string, Product>();

        public void SetProductList(Product[] stock)
        {
            productList.Clear();
            foreach(var p in stock)
            {
                if (productList.ContainsKey(p.Name))
                {
                    productList[p.Name].AddStock(p.Stock);
                } else
                {
                    productList.Add(p.Name, p);
                }
            }
        }

        public ProductBase GetProduct(string name)
        {
            if (OnRequestInfo != null)
                OnRequestInfo(RequestType.GetProduct);

            var products = from product in productList.Values
                           where product.Name == name
                           select product;

            if (products.Count() > 0) return products.First();

            throw new ProductNotFoundException();
        }

        private string uid = Guid.NewGuid().ToString();

        private readonly string name;
        public string StockManagerName
        {
            get
            {
                return name;
            }
        }

        public string GetUID()
        {
            return uid;
        }

        public bool HasProduct(string productName)
        {
            if (OnRequestInfo != null)
                OnRequestInfo(RequestType.HasProduct);

            return productList.ContainsKey(productName);
        }

        public bool HasProductFromFamily(string familyName)
        {
            if (OnRequestInfo != null)
                OnRequestInfo(RequestType.HasProductFromFamily);

            return (from product in productList.Values
                    where product.Family == familyName
                    select product).Count() > 0;
        }

        public string[] GetProductFamilies()
        {
            if (OnRequestInfo != null)
                OnRequestInfo(RequestType.GetProductFamilies);

            var families = from product in productList.Values
                           select product.Family;

            return families.Distinct().ToArray();
        }

        public ProductBase[] GetProducts(string family)
        {
            if(OnRequestInfo!=null)
                OnRequestInfo(RequestType.GetProducts);

            return (from product in productList.Values
                    where product.Family == family
                    select product).ToArray();
        }

        
        public delegate void RequestInfo(RequestType type);
        public event RequestInfo OnRequestInfo;

        public enum RequestType { GetProducts , GetProductFamilies , HasProductFromFamily , HasProduct, GetProduct }
    }
}
