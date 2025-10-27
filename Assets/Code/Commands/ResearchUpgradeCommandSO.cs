using Code.Players;
using Code.TechTree;
using Code.Units;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Research upgrade", menuName = "Tech/Research upgrade", order = 140)]
    public class ResearchUpgradeCommandSO : BaseCommandSO
    {
        [field: SerializeField] public UpgradeSO Upgrade { get; set; }
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding;
        }
        
        public override void Handle(CommandContext context)
        {
            BaseBuilding building = context.Commandable as BaseBuilding;
            if (UserSupplies.Instance != null && UserSupplies.Instance.HasEnoughSupplies(context.Owner, Upgrade.Cost))
            {
                building.BuildUnlockable(Upgrade);
            }
        }
        
        public override bool IsLocked(CommandContext context)
        {
            bool isLocked = !Upgrade.TechTree.IsUnlocked(context.Owner, Upgrade)
                            || (UserSupplies.Instance != null
                                && !UserSupplies.Instance.HasEnoughSupplies(context.Owner, Upgrade.Cost));
            return isLocked;
        }
    }
}