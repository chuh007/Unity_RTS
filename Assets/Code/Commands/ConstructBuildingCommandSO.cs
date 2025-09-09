using Code.Units;
using Code.Units.Data;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Construct building", menuName = "Units/Commands/Construct building", order = 0)]
    public class ConstructBuildingCommandSO : BaseCommandSO, IHasGhostPrefab
    {
        [field: SerializeField] public BuildingSO BuildingData { get; private set; }
        [field: SerializeField] public GameObject GhostPrefab { get; private set; }
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is IBuildingConstructor;
        }

        public override void Handle(CommandContext context)
        {
            IBuildingConstructor constructor = context.Commandable as IBuildingConstructor;
            constructor.Build(BuildingData, context.Hit.point);
        }
    }
}