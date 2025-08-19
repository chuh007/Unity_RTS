using Code.Units;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Code.Commands
{
    public struct CommandContext
    {
        public AbstractCommandable Commandable { get; private set; }
        public RaycastHit Hit { get; private set; }
        public MouseButton MouseButton { get; private set; }

        public CommandContext(AbstractCommandable commandable, RaycastHit hit, MouseButton mouseButton)
        {
            Commandable = commandable;
            Hit = hit;
            MouseButton = mouseButton;
        }
    }
}