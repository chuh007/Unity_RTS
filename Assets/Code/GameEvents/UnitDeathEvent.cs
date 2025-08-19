using Code.CoreSystem;
using Code.Units;

namespace Code.GameEvents
{
    public struct UnitDeathEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public UnitDeathEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}