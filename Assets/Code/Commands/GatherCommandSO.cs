using Code.Environments;
using Code.Units;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Gather command", menuName = "Units/Commands/Gather", order = 105)]
    public class GatherCommandSO : BaseCommandSO
    {
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is Worker
                   && context.Hit.collider != null
                   && context.Hit.collider.TryGetComponent(out GatherableSupply _);
        }

        public override void Handle(CommandContext context)
        {
            Worker worker = context.Commandable as Worker;
            worker.Gather(context.Hit.collider.GetComponent<GatherableSupply>());
        }
    }
}