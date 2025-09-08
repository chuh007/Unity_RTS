using System;
using System.Collections.Generic;
using System.Linq;
using Code.Commands;
using Code.CoreSystem;
using Code.GameEvents;
using Code.UI.Components;
using Code.Units;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UI.Containers
{
    public class CommandUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField] private CommandButtonUI[] commandButtons;
        
        public void EnableFor(HashSet<AbstractCommandable> selectedUnits)
        {
            RefreshButtons(selectedUnits);
        }

        public void Disable()
        {
            foreach (CommandButtonUI button in commandButtons)
            {
                button.Disable();
            }
        }
        
        private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits)
        {
            HashSet<BaseCommandSO> availableCommands = new HashSet<BaseCommandSO>();
            foreach (AbstractCommandable commandable in selectedUnits)
            {
                if (commandable.AvailableCommands != null)
                {
                    availableCommands.UnionWith(commandable.AvailableCommands);
                    //가능한 명령들을 합집합으로 합해준다.
                }
            }

            for (int i = 0; i < commandButtons.Length; i++)
            {
                BaseCommandSO commandForSlot = availableCommands.FirstOrDefault(action => action.Slot == i);

                if (commandForSlot != null)
                {
                    commandButtons[i].EnableFor(commandForSlot, HandleClick(commandForSlot));
                }
                else
                {
                    commandButtons[i].Disable();
                }
            }
        }

        private UnityAction HandleClick(BaseCommandSO commandForSlot)
            => () => Bus<CommandSelectEvent>.Raise(new CommandSelectEvent(commandForSlot));

        
    }
}