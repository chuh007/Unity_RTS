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

        //이 커맨드가 UI에 표기될 때 활성화할 것인지를 결정하는 함수다. 락되어있다면 true를 리턴한다.
        public abstract bool IsLocked(CommandContext context);
    }
}