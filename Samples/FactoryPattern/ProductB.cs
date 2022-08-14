using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.FactoryPattern
{
    internal class ProductB : IProduct
    {
        public ProductB(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
