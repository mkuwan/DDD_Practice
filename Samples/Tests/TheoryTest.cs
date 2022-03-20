using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Samples.Tests
{
    public class DeliveryService
    {
        public bool IsDeliveryValid(DateTime date)
        {
            if(date < DateTime.Now.Date)
                return false;

            return true;
        }

        public int CalculateMultiple(int x, int y)
        {
            return x * y;
        }

    }

    public class TheoryTest
    {
        public static List<object[]> Data()
        {
            return new List<object[]>
            {
                new object[] {DateTime.Now.AddDays(-1), false},
                new object[] {DateTime.Now, true},
                new object[] {DateTime.Now.AddDays(1), true},
            };
        }

        /// <summary>
        /// DateTime.Nowは.NETランタイムに依存しているため、許可されていません。
        /// この問題を克服する方法があります。xUnitには、テストメソッドにフィードするカスタムデータを生成するために使用できる別の機能[MemberData]があります
        /// </summary>
        /// <param name="deliveryDate"></param>
        /// <param name="expected"></param>
        [Theory]
        [MemberData(nameof(Data))]
        public void CanDelivery(DateTime deliveryDate, bool expected)
        {
            DeliveryService sut = new DeliveryService();
            bool isValid = sut.IsDeliveryValid(deliveryDate);

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(3, 3, 9)]
        [InlineData(4, 2, 8)]
        [InlineData(5, 6, 30)]
        [InlineData(6, 7, 42)]
        public void CalculateMultipleTest(int x, int y, int expected)
        {
            DeliveryService sut = new DeliveryService();
            int equals = sut.CalculateMultiple(x, y);

            Assert.Equal(expected, equals);
        }


    }
}
