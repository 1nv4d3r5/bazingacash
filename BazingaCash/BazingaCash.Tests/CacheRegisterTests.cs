using System;
using System.Collections.Generic;
using System.Linq;
using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Enums;
using BazingaCash.DomainModel.Interfaces;
using BazingaCash.Providers;
using BazingaCash.Providers.Extensions;
using NUnit.Framework;

namespace BazingaCash.Tests
{
    [TestFixture]
    public class CacheRegisterTests 
    {
        [Test]
        public void TestPurchaseProcess()
        {
            IDictionary<ProductType, double> productsPurchased =
                new Dictionary<ProductType, double>
                    {
                        {ProductType.Bread, 3.0}, // 3*2=6
                        {ProductType.Milk, 10.0}, // 10*3=30
                        {ProductType.Tomato, 2.25}, // 2.25*1 = 2.25  
                        {ProductType.Peanuts, 1.5} // 1.5*10=15
                    };
            const double expectedSumBeforeDiscount = 3 * 2 + 10 * 3 + 2.25 * 1 + 1.5 * 10;//53.25
            const double expectedSumAfterDiscount = expectedSumBeforeDiscount - 1 * 2 - 2 * 3 - 53.25 * 0.03;
            var coupons = new ICoupon[]
                              {
                                  new PercentOfTotalCoupon(3), // (53.25 - 7 ) * 0.03 = 1.3875
                                  new BuySomeGetFreeCoupon(2, 1, ProductType.Bread),//1*2=2
                                  new BuySomeGetFreeCoupon(3, 1, ProductType.Milk),//2*3=6
                                  new BuySomeGetFreeCoupon(10, 1, ProductType.Ananas)//not applicable
                              };

            ICashRegister cashRegister = new CashRegister(new InMemoryDataProvider(), new DiscountProvider());
            IPaymentProvider payments = new FakePaymentProvider();


            var productsToCountOrWeigh =
                cashRegister.ScanToGetQuanityTypes(productsPurchased.Keys);

            
            // count or weigh
            var productsWithoutPrice = productsToCountOrWeigh.Process(p => p.Amount = productsPurchased[p.Type]);

            // populate prices
            var products = cashRegister.PopulatePrices(productsWithoutPrice).ToArray();


            double priceSumBeforeDiscounts;
            double priceSumAfterDiscounts;
            IList<Discount> discounts;
            IList<ICoupon> usedCoupons;
            IList<ICoupon> notUsedCoupons;
            var canApplyDiscounts = 
                cashRegister.TryApplyDiscounts(products, coupons,
                                               out priceSumBeforeDiscounts,
                                               out priceSumAfterDiscounts,
                                               out discounts,
                                               out usedCoupons,
                                               out notUsedCoupons);

            // lock this products as in process of purchase
            var transactionId = cashRegister.BeginTransaction(products, discounts);

            if(payments.IsPaymentValid()) cashRegister.CommitTransaction(transactionId);
            else cashRegister.RollbackTransaction(transactionId);

            Assert.That(priceSumBeforeDiscounts, Is.EqualTo(expectedSumBeforeDiscount), "expectedSumBeforeDiscount");
            Assert.That(priceSumAfterDiscounts, Is.EqualTo(expectedSumAfterDiscount), "expectedSumAfterDiscount");
            Assert.That(canApplyDiscounts, Is.True);
            Assert.That(notUsedCoupons.Count, Is.EqualTo(1), "notUsedCoupons not as expected");
            Assert.That(notUsedCoupons[0].ToString(), Is.EqualTo("Buy 10 take 1 free Ananas"));
            Assert.That(usedCoupons.Count, Is.EqualTo(3), "usedCoupons not as expected");
            Assert.That(usedCoupons.Select(_ => _.ToString()).Contains("Buy 2 take 1 free Bread"), Is.True, "not found Buy 2 take 1 free Bread");
            Assert.That(usedCoupons.Select(_ => _.ToString()).Contains("Buy 3 take 1 free Milk"), Is.True, "not found Buy 3 take 1 free Milk");
            Assert.That(usedCoupons.Select(_ => _.ToString()).Contains("3 percent of all"), Is.True, "not found 3 percent of all");
        }
    }
}
