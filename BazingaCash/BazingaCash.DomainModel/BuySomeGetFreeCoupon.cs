using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BazingaCash.DomainModel.Enums;
using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.DomainModel
{
    public class BuySomeGetFreeCoupon : ICouponApplicableToSingleProduct
    {
        private readonly double _numToBuy;
        private readonly double _numGetFree;
        private readonly double _takeRatio;
        private readonly HashSet<ProductType> _appliesFor;

        public BuySomeGetFreeCoupon(double numToBuy, double numGetFree, params ProductType[] appliesFor)
        {
            _numToBuy = numToBuy;
            _numGetFree = numGetFree;

            if (_numToBuy <= 0 || _numGetFree <= 0)
            {
                throw new InvalidDataException("numToBuy or numGetFree cannot be less or equal to zero");
            }

            _takeRatio = _numGetFree / (double)(_numGetFree + _numToBuy);
            _appliesFor = new HashSet<ProductType>(appliesFor);
        }

        public bool AppliesTo(Product product)
        {
            return _appliesFor.Contains(product.Type) && product.Amount > _numToBuy;
        }

        public Discount GenerateDiscount(Product product)
        {
            var buyPlusTakeFree = _numToBuy + _numGetFree;
            var parts = product.Amount/buyPlusTakeFree;
            var partsRemainder = parts % 1.0;
            var partsWhole = parts - partsRemainder;

            return new Discount
                       {
                           Amount = Math.Round(
                                        partsWhole * _takeRatio * product.PriceForSingle * buyPlusTakeFree
                                        +
                                        Math.Max(partsRemainder - (1.0 - _takeRatio), 0.0) * product.PriceForSingle * buyPlusTakeFree,
                                        2
                                    ),
                            Coupon = this
                       };
        }
        public override string ToString()
        {
            return String.Format("Buy {0} take {1} free {2}", _numToBuy, _numGetFree, String.Join("|",_appliesFor.ToArray()));
        }
    }
}