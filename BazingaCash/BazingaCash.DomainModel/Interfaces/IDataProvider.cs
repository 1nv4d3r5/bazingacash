using System;
using System.Collections.Generic;
using BazingaCash.DomainModel.Enums;

namespace BazingaCash.DomainModel.Interfaces
{
    public interface IDataProvider
    {
        QuatityType GetQuatityTypeByProductType(ProductType productType);
        double GetPriceByProductType(ProductType type);
        Guid BeginTransaction(IEnumerable<Product> products, IEnumerable<Discount> discounts);
        void CommitTransaction(Guid transactionId);
        void RollbackTransaction(Guid transactionId);
    }
}