using System.Collections.Generic;

namespace BazingaCash.DomainModel.Interfaces
{
    public interface IDiscountProvider
    {
        bool TryApplyCoupons(
            IEnumerable<Product> products, 
            IEnumerable<ICoupon> coupons,
            out IList<Discount> discounts,
            out IList<ICoupon> usedCoupons,
            out IList<ICoupon> notUsedCoupons);
    }
}