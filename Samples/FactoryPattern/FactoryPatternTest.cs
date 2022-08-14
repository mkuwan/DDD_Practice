using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Samples.FactoryPattern
{
    public class FactoryPatternTest
    {
        [Fact]
        public void CreateFactoryTest()
        {
            // Arrange


            // Act
            var a = ProductFactory.ProductA("A");
            var b = ProductFactory.ProductB("B");

            // Assertion
            Assert.IsAssignableFrom<IProduct>(a);
            Assert.IsAssignableFrom<IProduct>(b);
            Assert.Equal("A", a.Name);
            Assert.Equal("B", b.Name);
        }
    }
}
