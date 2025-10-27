using Code.CoreSystem;
using Code.Units;

namespace Code.GameEvents
{
    public struct BuildingSpawnEvent : IEvent
    {
        public BaseBuilding Building { get; set; }
        public Owner Owner { get; }

        public BuildingSpawnEvent(Owner owner, BaseBuilding building)
        {
            Building = building;
            Owner = owner;
        }
    }
}