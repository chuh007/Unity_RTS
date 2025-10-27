using Code.Units;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Code.Commands
{
    public struct CommandContext
    {
        public AbstractCommandable Commandable { get; private set; } //누가 이 커맨드를 수행하는가?
        public RaycastHit Hit { get; private set; } //마우스 클릭시 클릭된 곳의 정보
        public int UnitIndex { get; private set; }
        public MouseButton MouseButton { get; private set; } //지금 좌클릭이냐? 우클릭이냐?
        public Owner Owner { get; private set; }

        public CommandContext(Owner owner)
        {
            Commandable = default;
            Hit = default;
            UnitIndex = default;
            MouseButton = default;
            Owner = owner;
        }
        
        public CommandContext(AbstractCommandable commandable, RaycastHit hit, int unitIndex = 0,
            MouseButton mouseButton = MouseButton.Left)
        {
            Commandable = commandable;
            Hit = hit;
            UnitIndex = unitIndex;
            MouseButton = mouseButton;
            Owner = Owner.Player;
        }
        
        public CommandContext(Owner owner, AbstractCommandable commandable, RaycastHit hit, int unitIndex = 0,
            MouseButton mouseButton = MouseButton.Left)
        {
            Commandable = commandable;
            Hit = hit;
            UnitIndex = unitIndex;
            MouseButton = mouseButton;
            Owner = owner;
        }
    }
}