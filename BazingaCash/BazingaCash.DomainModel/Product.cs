using BazingaCash.DomainModel.Enums;

namespace BazingaCash.DomainModel
{
    public class Product
    {
        public ProductType Type { get; set; }
        public QuatityType QuatityType { get; set; }
        public double Amount { get; set; }
        public double PriceForSingle { get; set; }
        public double Price { get; set; }
    }
}
