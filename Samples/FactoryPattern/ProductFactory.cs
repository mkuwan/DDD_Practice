using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.FactoryPattern
{
    /// <summary>
    /// Factory Methodの特徴は、クラスのインスタンス化を行う部分を同じFactoryクラスに集約することで、
    /// クライアント（そのクラスのインスタンスを必要としている部分）がクラスに直接依存せずに済むということです
    /// </summary>
    internal static class ProductFactory
    {
        public static IProduct ProductA(string name) => new ProductA(name);

        public static IProduct ProductB(string name) => new ProductB(name);
    }
}
