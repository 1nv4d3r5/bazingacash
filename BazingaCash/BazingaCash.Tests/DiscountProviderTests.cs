using System.Collections.Generic;
using System.Linq;
using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Enums;
using BazingaCash.DomainModel.Interfaces;
using BazingaCash.Providers;
using NUnit.Framework;

namespace BazingaCash.Tests
{
    [TestFixture]
    public class DiscountProviderTests
    {
        [Test]
        public void CanApplyCoupons()
        {
            IDiscountProvider discountsProvider = new DiscountProvider();
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
            var coupons =
                new[]
                    {
                        new BuySomeGetFreeCoupon(5, 2, ProductType.Milk),
                        new BuySomeGetFreeCoupon(1.5, 0.5, ProductType.Peanuts)
                    };

            IList<Discount> discounts;
            IList<ICoupon> usedCoupons;
            IList<ICoupon> notUsedCoupons;
            var result = 
                discountsProvider.TryApplyCoupons(products, coupons, out discounts, out usedCoupons, out notUsedCoupons);

            Assert.That(result, Is.True);
            Assert.That(notUsedCoupons, Is.Empty);
// ReSharper disable CompareOfFloatsByEqualityOperator
            Assert.That(discounts.Any(d => d.Amount == 4.0), Is.True);// buy 5 take 2 free, 2 * 2 = 4
            Assert.That(discounts.Any(d => d.Amount == 5.0), Is.True);// buy 1.5 take 0.5 free, 10 * 0.5 = 5
// ReSharper restore CompareOfFloatsByEqualityOperator
        }
    }
}