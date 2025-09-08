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
            return context.Commandable is BaseBuilding {QueueSize: < BaseBuilding.MAX_QUEUE_SIZE};
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = context.Commandable as BaseBuilding;
            
            building.BuildUnit(Unit); //나중엔 조금더 복잡해진다.
        }
    }
}