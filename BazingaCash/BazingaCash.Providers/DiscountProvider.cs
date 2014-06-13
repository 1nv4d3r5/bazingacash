using System;
using System.Linq;
using System.Collections.Generic;
using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.Providers
{
    public class DiscountProvider : IDiscountProvider
    {
        bool IDiscountProvider.TryApplyCoupons(
            IEnumerable<Product> products, 
            IEnumerable<ICoupon> coupons,
            out IList<Discount> discounts,
            out IList<ICoupon> usedCoupons,
            out IList<ICoupon> notUsedCoupons)
        {
            discounts = null;
            usedCoupons = notUsedCoupons = null;
            if (coupons == null) return false;
            // split single product and multiple product coupons
            var couponsList = coupons.ToList();
            var couponsTuple = 
                couponsList.Aggregate(Tuple.Create(new List<ICouponApplicableToSingleProduct>(), new List<ICouponApplicableToMultipleProducts>()), 
                (t, c) =>
                    {
                        var sing = c as ICouponApplicableToSingleProduct;
                        if(sing != null)
                        {
                            t.Item1.Add(sing);
                        } 
                        else
                        {
                            var mult = c as ICouponApplicableToMultipleProducts;
                            if (mult != null)
                            {
                                t.Item2.Add(mult);
                            } 
                        }
                        return t;
                    });

            
            var productsList = products.ToList();
            discounts = new List<Discount>();
            var usedSet = new HashSet<ICoupon>();
          

            // try apply single product coupons
            foreach (var product in productsList)
            {
                foreach (var coupon in couponsTuple.Item1)
                {
                    if(coupon.AppliesTo(product))
                    {
                        var discount = coupon.GenerateDiscount(product);
                        usedSet.Add(coupon);
                        discounts.Add(discount);
                    }
                }
            }

            // try apply multiple product coupons
            foreach (var coupon in couponsTuple.Item2)
            {
                if (coupon.AppliesTo(productsList))
                {
                    var discount = coupon.GenerateDiscount(productsList);
                    usedSet.Add(coupon);
                    discounts.Add(discount);
                }
            }

            notUsedCoupons = new List<ICoupon>(couponsList.Where(c => !usedSet.Contains(c)));
            usedCoupons = new List<ICoupon>(usedSet);
            return true;
        }
    }
}