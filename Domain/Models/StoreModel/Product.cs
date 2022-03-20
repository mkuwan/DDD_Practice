using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.SeedWork;

namespace Domain.Models.StoreModel
{
    public sealed class Product: EntityStringId
    {
        public Product(string id, string productName, decimal unitPrice)
        {
            Id = id;
            ProductName = productName;
            UnitPrice = unitPrice;
        }

        //private readonly string _productId = Guid.NewGuid().ToString();

        //public string ProductId
        //{
        //    get => _productId;
        //    init
        //    {
        //        if (String.IsNullOrWhiteSpace(_productId))
        //            value = Guid.NewGuid().ToString("D");
        //        _productId = value;
        //    }
        //}

        public string ProductName { get; init; }

        public decimal UnitPrice { get; init; }
    }
}
