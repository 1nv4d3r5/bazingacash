using System;
using System.Collections.Generic;
using BazingaCash.DomainModel.Enums;

namespace BazingaCash.DomainModel.Interfaces
{
    public interface ICashRegister
    {
        IEnumerable<Product> ScanToGetQuanityTypes(IEnumerable<ProductType> productTypes);
        IEnumerable<Product> PopulatePrices(IEnumerable<Product> products);
        bool TryApplyDiscounts(
            IEnumerable<Product> products, 
            IEnumerable<ICoupon> coupons, 
            out double priceSumBeforeDiscounts, 
            out double priceSumAfterDiscounts, 
            out IList<Discount> discounts, 
            out IList<ICoupon> usedCoupons, 
            out IList<ICoupon> notUsedCoupons);

        Guid BeginTransaction(IEnumerable<Product> products, IEnumerable<Discount> discounts);
        void CommitTransaction(Guid transactionId);
        void RollbackTransaction(Guid transactionId);
    }
}