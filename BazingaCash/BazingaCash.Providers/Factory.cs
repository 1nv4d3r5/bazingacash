using BazingaCash.DomainModel.Interfaces;
using Microsoft.Practices.Unity;

namespace BazingaCash.Providers
{
    public class Factory : IFactory
    {
        private static readonly IFactory Get = new Factory();
        private readonly IUnityContainer _container = new UnityContainer();

        private Factory()
        {
            SetUpDefaults();
            ApplyConfigurationOverrides();
        }

        ICashRegister IFactory.CashRegister { get { return _container.Resolve<ICashRegister>(); } }

        private void SetUpDefaults()
        {
            _container.RegisterType<ICashRegister, CashRegister>(Singleton);
            _container.RegisterType<IDataProvider, DataProvider>(Singleton);
        }
        private void ApplyConfigurationOverrides()
        {
            // TODO: implement configurations if needed here ...
        }
        private LifetimeManager Singleton { get { return new ContainerControlledLifetimeManager(); } }
    }
}