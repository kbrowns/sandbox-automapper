using System;
using System.Linq;
using AutoMapper;
using NUnit.Framework;

namespace Sandbox_AutoMapper
{
    [SetUpFixture]
    public class SetupFixture
    {
        [SetUp]
        public void Setup()
        {
            AutoMapperConfiguration.Configure();
        }
    }

    [TestFixture]
    public class Class1
    {
        [Test]
        public void Map_Bidirectionally_With_ForMember()
        {
            var summary = Mapper.Map<Order, OrderSummary>(new Order {OrderNumber = "1234"});
            Assert.That("1234", Is.EqualTo(summary.OrderNum));

            var order = Mapper.Map<OrderSummary, Order>(summary);
            Assert.That(summary.OrderNum, Is.EqualTo(order.OrderNumber));
        }
    }

    public class Order
    {
        public string OrderNumber { get; set; }
    }

    public class OrderSummary
    {
        public string OrderNum { get; set; }
    }

    public class OrderingMap : Profile
    {
        protected override void Configure()
        {
            CreateMap<Order, OrderSummary>()
                .ForMember(dest => dest.OrderNum, opt => opt.MapFrom(src => src.OrderNumber))
                .ReverseMap()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNum));
        }
    }

    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x => GetConfiguration(Mapper.Configuration));
        }

        private static void GetConfiguration(IConfiguration configuration)
        {
            var profiles = typeof(OrderingMap).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x));
            foreach (var profile in profiles)
            {
                configuration.AddProfile(Activator.CreateInstance(profile) as Profile);
            }
        }
    }
}
