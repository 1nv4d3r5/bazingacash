using BazingaCash.DomainModel.Interfaces;
using Microsoft.Practices.Unity;

namespace BazingaCash.Providers
{
    public class Factory : IFactory
    {
        private readonly IUnityContainer _container = new UnityContainer();

        public Factory()
        {
            SetUpDefaults();
            ApplyConfigurationOverrides();
        }

        ICashRegister IFactory.CashRegister { get { return _container.Resolve<ICashRegister>(); } }

        private void SetUpDefaults()
        {
            _container.RegisterType<ICashRegister, CashRegister>(Singleton);
            _container.RegisterType<IDiscountProvider, DiscountProvider>(Singleton);
            _container.RegisterType<IDataProvider, InMemoryDataProvider>(Singleton);
        }
        private void ApplyConfigurationOverrides()
        {
            // TODO: implement configurations if needed here ...
        }
        private LifetimeManager Singleton { get { return new ContainerControlledLifetimeManager(); } }
    }
}