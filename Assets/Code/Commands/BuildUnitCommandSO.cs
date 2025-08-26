using Code.Units;
using Code.Units.Data;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Build unit", menuName = "Buildings/ Commands/ Build unit", order = 150)]
    public class BuildUnitCommandSO : BaseCommandSO
    {
        [field: SerializeField] public UnitSO Unit { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding; // 빌딩이면 유닛 생산 가능
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = context.Commandable as BaseBuilding;
            
            building.BuildUnit(Unit);
        }
    }
}