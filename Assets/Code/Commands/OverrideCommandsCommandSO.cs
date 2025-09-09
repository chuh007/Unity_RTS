using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Override command", menuName = "Units/Commands/Override", order = 110)]
    public class OverrideCommandsCommandSO : BaseCommandSO
    {
        [field: SerializeField] public BaseCommandSO[] Commands { get; private set; }
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is not null;
        }

        public override void Handle(CommandContext context)
        {
            context.Commandable.SetCommandOverrides(Commands);
        }
    }
}