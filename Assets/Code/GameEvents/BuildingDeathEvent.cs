using Code.CoreSystem;
using Code.Units;

namespace Code.GameEvents
{
    public struct BuildingDeathEvent : IEvent
    {
        public BaseBuilding Building { get; }
        public Owner Owner { get; }
        
        public BuildingDeathEvent(Owner owner, BaseBuilding building)
        {
            Building = building;
            Owner = building.Owner;
        }
    }
}