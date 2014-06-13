using System;
using System.Collections.Generic;
using System.Data;
using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Enums;
using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.Providers
{
    public class InMemoryDataProvider : IDataProvider
    {
        private readonly IDictionary<ProductType, QuatityType> TableQuatityTypeByProductType =
            new Dictionary<ProductType, QuatityType>
                {
                    { ProductType.Bread, QuatityType.Countable },
                    { ProductType.Milk, QuatityType.Countable },
                    { ProductType.Tomato, QuatityType.Weighable },
                    { ProductType.Peanuts, QuatityType.Weighable }
                };
        private readonly IDictionary<ProductType, double> TablePriceByProductType =
            new Dictionary<ProductType, double>
                {
                    { ProductType.Bread, 2.0 },
                    { ProductType.Milk, 3.0 },
                    { ProductType.Tomato, 1.0 },
                    { ProductType.Peanuts, 10.0 }
                };
        QuatityType IDataProvider.GetQuatityTypeByProductType(ProductType productType)
        {
            QuatityType qt;
            if (!TableQuatityTypeByProductType.TryGetValue(productType, out qt))
            {
                throw new DataException("Missing Quatity Type information about product with type " + productType);
            }
            return qt;
        }

        double IDataProvider.GetPriceByProductType(ProductType productType)
        {
            double price;
            if (!TablePriceByProductType.TryGetValue(productType, out price))
            {
                throw new DataException("Missing price information about product with type " + productType);
            }
            return price;
        }

        Guid IDataProvider.BeginTransaction(IEnumerable<Product> products, IEnumerable<Discount> discounts)
        {
            // TODO: call database to insert transaction record
            return Guid.NewGuid();
        }
        void IDataProvider.CommitTransaction(Guid transactionId)
        {
            // TODO: call database to commit transaction
        }
        void IDataProvider.RollbackTransaction(Guid transactionId)
        {
            // TODO: call database to rollback transaction
        }
    }
}