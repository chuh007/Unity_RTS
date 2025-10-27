using System;
using System.Collections.Generic;
using System.Linq;
using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using Code.Players;
using Code.UI.Containers;
using Code.Units;
using Code.Units.Buildings;
using UnityEngine;

namespace Code.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private CommandUI commandUI;
        //[SerializeField] private GenerateUnitUI generateUnitUI;
        [SerializeField] private BuildingSelectedUI buildingSelectedUI;
        
        [SerializeField] private SupplyUI supplyUI;
        
        [SerializeField] private UnitIconUI unitIconUI;
        [SerializeField] private SingleUnitSelectUI singleUnitSelectUI;
        [SerializeField] private Owner uiOwner = Owner.Player;
        
        private HashSet<AbstractCommandable> _selectedUnits = new HashSet<AbstractCommandable>(12);

        private void Awake()
        {
            Bus<UnitSelectEvent>.OnEvents[uiOwner] += HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvents[uiOwner] += HandleUnitDeselect;
            Bus<UnitDeathEvent>.OnEvents[uiOwner] += HandleUnitDeath;
            //Bus<SupplyEvent>.OnEvents[uiOwner] += HandleSupplyChange;
        }

        private void Start()
        {
            DisableAllUI();
            supplyUI.EnableFor(UserSupplies.Instance);
            UserSupplies.Instance.OnSupplyChanged += HandleSupplyChange;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectEvent>.OnEvents[uiOwner] -= HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvents[uiOwner] -= HandleUnitDeselect;
            Bus<UnitDeathEvent>.OnEvents[uiOwner] -= HandleUnitDeath;
            //Bus<SupplyEvent>.OnEvents[uiOwner] -= HandleSupplyChange;
            UserSupplies.Instance.OnSupplyChanged -= HandleSupplyChange;
        }

        private void HandleSupplyChange(Owner owner,  int amount, SupplySO supplyType)
        {
            if(owner != uiOwner) return;
            commandUI.EnableFor(_selectedUnits);
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            _selectedUnits.Remove(evt.Unit);
            RefreshUI();
        }

        private void HandleUnitSelect(UnitSelectEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _selectedUnits.Add(commandable);
            }
            RefreshUI();
        }

        private void HandleUnitDeselect(UnitDeselectEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _selectedUnits.Remove(commandable);
            }
            RefreshUI();
        }

        private void RefreshUI()
        {
            DisableAllUI();
            if (_selectedUnits.Count > 0)
            {
                commandUI.EnableFor(_selectedUnits);

                if (_selectedUnits.Count == 1)
                {
                    AbstractCommandable firstUnit = _selectedUnits.First();
                    unitIconUI.EnableFor(firstUnit);
                    if (firstUnit is IBuilding building)
                    {
                        buildingSelectedUI.EnableFor(building);
                    }
                    else
                    {
                        singleUnitSelectUI.EnableFor(firstUnit);
                    }
                }
            }
        }

        private void DisableAllUI()
        {
            commandUI.Disable();
            buildingSelectedUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectUI.Disable();
        }
    }
}