using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using Xunit;

namespace VitalChoice.Tests
{
    public class CollectionMethodsTests
    {
        [Fact]
        public void TestGroupByTakeLast()
        {
            var data = new List<CustomerCardData>();

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 3,
                CardNumber = "Test1"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 3,
                CardNumber = "Test2"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 3,
                CardNumber = "Test3"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 5,
                CardNumber = "Test5"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 4,
                CardNumber = "Test4"
            });

            Assert.Equal(data.GroupByTakeLast(c => c.IdPaymentMethod).Single(p => p.IdPaymentMethod == 3).CardNumber, "Test3");
            Assert.Equal(data.GroupByTakeLast(c => c.IdPaymentMethod).Single(p => p.IdPaymentMethod == 5).CardNumber, "Test5");
            Assert.Equal(data.GroupByTakeLast(c => c.IdPaymentMethod).Single(p => p.IdPaymentMethod == 4).CardNumber, "Test4");
        }

        [Fact]
        public void TestExceptKeyedWith()
        {
            var data = new List<CustomerCardData>();

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 3,
                CardNumber = "Test1"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 3,
                CardNumber = "Test2"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 3,
                CardNumber = "Test3"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 5,
                CardNumber = "Test5"
            });

            data.Add(new CustomerCardData
            {
                IdCustomer = 5,
                IdPaymentMethod = 4,
                CardNumber = "Test4"
            });

            Assert.Equal(data.ExceptKeyedWith(Enumerable.Empty<CustomerCardData>(), cardData => cardData.IdPaymentMethod).Count(), 5);
            Assert.Equal(data.ExceptKeyedWith(Enumerable.Empty<int>(), cardData => cardData.IdPaymentMethod, i => i).Count(), 5);
            Assert.Equal(data.ExceptKeyedWith(Enumerable.Empty<int>(), cardData => cardData.IdPaymentMethod, i => i).Count(), 5);

            Assert.Equal(data.ExceptKeyedWith(new[] {new CustomerCardData
            {
                IdPaymentMethod = 3
            } }, cardData => cardData.IdPaymentMethod).Count(), 2);

            Assert.Equal(data.ExceptKeyedWith(new[]
            {
                new CustomerCardData
                {
                    IdPaymentMethod = 5
                }
            }, cardData => cardData.IdPaymentMethod).Count(), 4);

            Assert.Equal(data.ExceptKeyedWith(new[]
            {
                new CustomerCardData
                {
                    IdPaymentMethod = 89
                }
            }, cardData => cardData.IdPaymentMethod).Count(), 5);
        }
    }
}
