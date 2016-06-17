using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Interfaces.Services.Orders;
using Xunit;

namespace VitalChoice.Tests
{
    public class DummyTest
    {
        [Fact]
        public void Test()
        {
            var serviceProvider = TestsConfig.Host.Services;
            var orderService = serviceProvider.GetRequiredService<IOrderService>();
            Assert.NotNull(orderService);
        }
    }
}
