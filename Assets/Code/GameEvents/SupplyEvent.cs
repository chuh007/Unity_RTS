using Code.CoreSystem;
using Code.Environments;

namespace Code.GameEvents
{
    public struct SupplyEvent : IEvent
    {
        public int Amount { get; private set; }
        public SupplySO SupplyData { get; private set; }

        public SupplyEvent(int amount, SupplySO supplyData)
        {
            Amount = amount;
            SupplyData = supplyData;
        }
    }
}