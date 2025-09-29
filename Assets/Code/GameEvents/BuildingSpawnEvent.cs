using Code.CoreSystem;
using Code.Units;

namespace Code.GameEvents
{
    public struct BuildingSpawnEvent : IEvent
    {
        public BaseBuilding Building { get; private set; }

        public BuildingSpawnEvent(BaseBuilding building)
        {
            Building = building;
        }
    }
}