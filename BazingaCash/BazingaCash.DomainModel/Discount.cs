using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.DomainModel
{
    public class Discount
    {
        public double Amount { get; set; }
        public ICoupon Coupon { get; set; }
    }
}