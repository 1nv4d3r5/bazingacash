using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Enums;
using NUnit.Framework;

namespace BazingaCash.Tests
{
    [TestFixture]
    public class PercentOfTotalCouponTests
    {
        [Test]
        public void CanCalculatePercentOfTotal()
        {
            var products =
                new[]
                    {
                        new Product
                            {
                                Amount = 10,
                                PriceForSingle = 2,
                                Price = 20,
                                QuatityType = QuatityType.Countable,
                                Type = ProductType.Milk
                            },
                        new Product
                            {
                                Amount = 3.5,
                                PriceForSingle = 10,
                                Price = 35,
                                QuatityType = QuatityType.Weighable,
                                Type = ProductType.Peanuts
                            }
                    };

            var discount = new PercentOfTotalCoupon(7.5).GenerateDiscount(products);

            Assert.That(discount, Is.Not.Null, "Expected discount");
            Assert.That(discount.Amount, Is.EqualTo((10 * 2 + 3.5*10)*0.075), "Expected discount");
        }
    }
}