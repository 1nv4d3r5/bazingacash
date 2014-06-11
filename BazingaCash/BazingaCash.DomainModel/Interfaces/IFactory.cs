namespace BazingaCash.DomainModel.Interfaces
{
    public interface IFactory
    {
        ICashRegister CashRegister { get; }
    }
}