using System.Collections.Generic;
using System.Linq;
using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.DomainModel
{
    public class PercentOfTotalCoupon : ICouponApplicableToMultipleProducts
    {
        private readonly double _partOfTotal;

        public PercentOfTotalCoupon(double percentageOfTotal)
        {
            _partOfTotal = percentageOfTotal/100.0;
        }

        public bool AppliesTo(IEnumerable<Product> products)
        {
            return products.Sum(p => p.Amount * p.PriceForSingle) > 0;
        }

        public Discount GenerateDiscount(IEnumerable<Product> products)
        {
            return new Discount { Amount = products.Sum(p => p.Amount * p.PriceForSingle) * _partOfTotal, Coupon = this };
        }
        public override string ToString()
        {
            return (_partOfTotal * 100) + " percent of all";
        }
    }
}