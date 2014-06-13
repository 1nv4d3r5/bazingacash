using BazingaCash.DomainModel.Interfaces;

namespace BazingaCash.Providers
{
    public class FakePaymentProvider : IPaymentProvider
    {
         bool IPaymentProvider.IsPaymentValid()
         {
             return true;
         }
    }
}