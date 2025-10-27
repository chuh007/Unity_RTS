using Code.CoreSystem;
using Code.Environments;
using Code.Units;

namespace Code.GameEvents
{
    public struct SupplyEvent : IEvent
    {
        public Owner Owner { get; }
        public int Amount { get;}
        public SupplySO SupplyData { get;}

        public SupplyEvent(Owner owner, int amount, SupplySO supplyData)
        {
            Owner = owner;
            Amount = amount;
            SupplyData = supplyData;
        }
    }
}