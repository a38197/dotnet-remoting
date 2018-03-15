using System;
using SuperSoftware.Shared;

namespace SuperSoftware.Client
{
    [Serializable]
    public class Product : ProductBase
    {
        private Product() { }

        public override string Name
        {
            get; set;
        }

        public override string Family
        {
            get; set;
        }

        public override int Stock
        {
            get; set;
        }

        public override decimal Price
        {
            get; set;
        }

        public Product(string name, string family)
        {
            Family = family;
            Name = name;
            Stock = 0;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}", Family, Name);
        }

        public void AddStock(int value)
        {
            Stock += value;
        }
    }
}
