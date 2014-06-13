using System;
using System.Collections.Generic;
using System.Linq;
using BazingaCash.DomainModel;
using BazingaCash.DomainModel.Enums;
using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.Providers
{
    public class CashRegister : ICashRegister
    {
        private readonly IDataProvider _data;
        private readonly IDiscountProvider _discounts;

        public CashRegister(IDataProvider data, IDiscountProvider discounts)
        {
            _data = data;
            _discounts = discounts;
        }

        IEnumerable<Product> ICashRegister.ScanToGetQuanityTypes(IEnumerable<ProductType> productTypes)
        {
            return productTypes.Select(pt => 
                    new Product
                        {
                            Type = pt, 
                            QuatityType = _data.GetQuatityTypeByProductType(pt)
                        });
        }
        IEnumerable<Product> ICashRegister.PopulatePrices(IEnumerable<Product> products)
        {
            var en = products.GetEnumerator();
            while (en.MoveNext())
            {
                var p = en.Current;
                p.PriceForSingle = _data.GetPriceByProductType(p.Type);
                p.Price = p.PriceForSingle*p.Amount;
                yield return p;
            }
        }
        bool ICashRegister.TryApplyDiscounts(
            IEnumerable<Product> products,
            IEnumerable<ICoupon> coupons,
            out double priceSumBeforeDiscounts,
            out double priceSumAfterDiscounts,
            out IList<Discount> discounts,
            out IList<ICoupon> usedCoupons,
            out IList<ICoupon> notUsedCoupons)
        {
            var productsList = products.ToList();
            priceSumAfterDiscounts = priceSumBeforeDiscounts = productsList.Sum(p => p.Price);
            if (_discounts.TryApplyCoupons(productsList, coupons, out discounts, out usedCoupons, out notUsedCoupons))
            {
                priceSumAfterDiscounts = priceSumBeforeDiscounts - discounts.Sum(d => d.Amount);
                return true;
            }
            return false;
        }

        Guid ICashRegister.BeginTransaction(IEnumerable<Product> products, IEnumerable<Discount> discounts)
        {
            return _data.BeginTransaction(products, discounts);
        }

        void ICashRegister.CommitTransaction(Guid transactionId)
        {
            _data.CommitTransaction(transactionId);
        }

        void ICashRegister.RollbackTransaction(Guid transactionId)
        {
            _data.RollbackTransaction(transactionId);
        }
    }
}
