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

namespace Code.Containers
{
    public class CommandUI : MonoBehaviour
    {
        [SerializeField] private CommandButtonUI[] commandButtons;
        
        private HashSet<AbstractCommandable> _selectedUnits = new HashSet<AbstractCommandable>();

        private void Awake()
        {
            Bus<UnitSelectEvent>.OnEvent += HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent += HandleUnitDeSelect;
        }

        private void Start()
        {
            foreach (CommandButtonUI buttonUI in commandButtons)
            {
                buttonUI.Disable();
            }
        }

        private void OnDestroy()
        {
            Bus<UnitSelectEvent>.OnEvent -= HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent -= HandleUnitDeSelect;
        }

        private void HandleUnitSelect(UnitSelectEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _selectedUnits.Add(commandable);
                RefreshButtons();
            }
        }
        
        private void HandleUnitDeSelect(UnitDeselectEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _selectedUnits.Remove(commandable);
                RefreshButtons();
            }
        }
        
        private void RefreshButtons()
        {
            HashSet<BaseCommandSO> availableCommands = new HashSet<BaseCommandSO>();
            foreach (AbstractCommandable commandable in _selectedUnits)
            {
                if (commandable.AvailableCommands != null)
                {
                    availableCommands.UnionWith(commandable.AvailableCommands);
                }
            }

            for (int i = 0; i < commandButtons.Length; i++)
            {
                BaseCommandSO commandForSlot = availableCommands.FirstOrDefault(action => action.Slot == i);
                
                if (commandForSlot != null)
                {
                    commandButtons[i].EnableFor(commandForSlot, HandleClink(commandForSlot));
                }
                else
                {
                    commandButtons[i].Disable();
                }
            }
        }

        private UnityAction HandleClink(BaseCommandSO commandForSlot)
            => () => Bus<CommandSelectEvent>.Raise(new CommandSelectEvent(commandForSlot));
    }
}