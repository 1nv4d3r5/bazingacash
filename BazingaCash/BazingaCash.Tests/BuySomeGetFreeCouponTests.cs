using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Enums;
using NUnit.Framework;

namespace BazingaCash.Tests
{
    [TestFixture]
    public class BuySomeGetFreeCouponTests
    {
        [Test]
        [TestCase(4, 2, 3, 1, 0)]
        [TestCase(4, 2, 6, 1, 2)]
        [TestCase(4, 2, 10, 1, 2)]
        [TestCase(4, 2, 11, 1, 3)]
        [TestCase(4, 2, 11, 5, 15)]
        [TestCase(3, 1, 10, 1, 2)]
        [TestCase(3, 2, 14, 1, 5)]
        [TestCase(3, 2, 14, 0.5, 2.5)]
        [TestCase(1.5, 0.5, 7, 1, 1.5)]
        public void CanGenerateDiscount(double buy, double getFree, double amount, double priceForSingle, double expectedDiscount)
        {
            var discount =
                new BuySomeGetFreeCoupon(buy, getFree, ProductType.Bread)
                    .GenerateDiscount(new Product
                                          {
                                              Type = ProductType.Bread,
                                              Amount = amount,
                                              PriceForSingle = priceForSingle,
                                              Price = priceForSingle*amount
                                          });

            Assert.That(discount.Amount, Is.EqualTo(expectedDiscount));
        }

    }
}