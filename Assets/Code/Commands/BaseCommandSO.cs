using System.Data;
using UnityEngine;

namespace Code.Commands
{
    public abstract class BaseCommandSO : ScriptableObject, ICommand
    {
        [field: SerializeField] public string Name { get; private set; } = "Command";
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: Range(-1, 8)]
        [field: SerializeField] public int Slot { get; private set; }
        [field: SerializeField] public bool IsSingleUnitCommand { get; private set; }
        [field: SerializeField] public bool RequireClickToActivate { get; private set; } = true;
        
        public abstract bool CanHandle(CommandContext context);
        public abstract void Handle(CommandContext context);
    }
}