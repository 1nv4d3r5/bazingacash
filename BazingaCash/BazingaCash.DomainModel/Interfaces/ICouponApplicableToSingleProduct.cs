namespace BazingaCash.DomainModel.Interfaces
{
    public interface ICouponApplicableToSingleProduct : ICoupon
    {
        bool AppliesTo(Product product);
        Discount GenerateDiscount(Product product);
    }
}