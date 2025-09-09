using Code.Environments;
using Code.Units;
using Code.Units.Data;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Gather command", menuName = "Units/Commands/Gather", order = 105)]
    public class GatherCommandSO : BaseCommandSO
    {
        [SerializeField] private BuildingSO headQuarterSO;
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is Worker
                   && context.Hit.collider != null
                   && IsGatherableSupplyOrHeadQuarter(context.Hit.collider);
        }

        public override void Handle(CommandContext context)
        {
            Worker worker = context.Commandable as Worker;
            
            if(context.Hit.collider.TryGetComponent(out GatherableSupply gatherable))
                worker.Gather(gatherable);
            else if (IsHeadQuarter(context.Hit.collider) && worker.HasSupplies)
                worker.ReturnSupplies(context.Hit.collider.gameObject);
            else
                worker.MoveTo(context.Hit.collider.transform.position);
        }

        private bool IsGatherableSupplyOrHeadQuarter(Collider collider)
            => collider.TryGetComponent(out GatherableSupply _) || IsHeadQuarter(collider);
        
        private bool IsHeadQuarter(Collider collider)
            => collider.TryGetComponent(out BaseBuilding building) && building.UnitSo.Equals(headQuarterSO);
    }
}