using Code.Players;
using Code.Units;
using Code.Units.Data;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Build unit", menuName = "Buildings/Commands/Build unit", order = 120)]
    public class BuildUnitCommandSO : BaseCommandSO
    {
        //빌딩이 생산할 유닛 정보
        [field: SerializeField] public UnitSO Unit { get; private set; }
        
        public override bool CanHandle(CommandContext context)
        {
            //빌딩이면 유닛을 생산할 수 있다.
            return context.Commandable is BaseBuilding {QueueSize: < BaseBuilding.MAX_QUEUE_SIZE}
                   && UserSupplies.Instance != null
                   && UserSupplies.Instance.HasEnoughSupplies(context.Owner, Unit.Cost);
        }

        public override void Handle(CommandContext context)
        {
            if (UserSupplies.Instance.HasEnoughSupplies(context.Owner, Unit.Cost) == false) return;
            
            BaseBuilding building = context.Commandable as BaseBuilding;
            
            building.BuildUnlockable(Unit); //나중엔 조금더 복잡해진다.
        }
        
        public override bool IsLocked(CommandContext context) 
            => !Unit.TechTree.IsUnlocked(context.Owner, Unit) ||
               (UserSupplies.Instance != null 
                && !UserSupplies.Instance.HasEnoughSupplies(context.Owner, Unit.Cost));
    }
}