using Code.CoreSystem;
using Code.TechTree;
using Code.Units;

namespace Code.GameEvents
{
    public struct UpgradeResearchedEvent : IEvent
    {
        public Owner Owner { get;}
        public UpgradeSO Upgrade { get; }

        public UpgradeResearchedEvent(Owner owner, UpgradeSO upgrade)
        {
            Owner = owner;
            Upgrade = upgrade;
        }
    }
}