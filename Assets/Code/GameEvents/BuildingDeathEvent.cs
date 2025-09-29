using Code.CoreSystem;
using Code.Units;

namespace Code.GameEvents
{
    public struct BuildingDeathEvent : IEvent
    {
        public BaseBuilding Building { get; private set; }

        public BuildingDeathEvent(BaseBuilding building)
        {
            Building = building;
        }
    }
}