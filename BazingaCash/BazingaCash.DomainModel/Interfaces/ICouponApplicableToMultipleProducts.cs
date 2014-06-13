using System.Collections.Generic;

namespace BazingaCash.DomainModel.Interfaces
{
    public interface ICouponApplicableToMultipleProducts : ICoupon
    {
        bool AppliesTo(IEnumerable<Product> products);
        Discount GenerateDiscount(IEnumerable<Product> products); 
    }
}