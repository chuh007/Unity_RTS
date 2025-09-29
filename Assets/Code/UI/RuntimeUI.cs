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
using Code.Units.Data;
using UnityEngine;

namespace Code.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private CommandUI commandUI;
        // [SerializeField] private GenerateUnitUI generateUnitUI;
        [SerializeField] private BuildingSelectedUI buildingSelectedUI;
        
        [SerializeField] private SupplyUI supplyUI;
        
        [SerializeField] private UnitIconUI unitIconUI;
        [SerializeField] private SingleUnitSelectUI singleUnitSelectUI;
        
        private HashSet<AbstractCommandable> _selectedUnits = new HashSet<AbstractCommandable>(12);

        private void Awake()
        {
            Bus<UnitSelectEvent>.OnEvent += HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent += HandleUnitDeselect;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            // Bus<SupplyEvent>.OnEvent += HandleSupplyChange;
        }

        private void Start()
        {
            DisableAllUI();
            supplyUI.EnableFor(UserSupplies.Instance);
            UserSupplies.Instance.OnSupplyChanged += HandleSupplyChange;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectEvent>.OnEvent -= HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent -= HandleUnitDeselect;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            // Bus<SupplyEvent>.OnEvent -= HandleSupplyChange;
            UserSupplies.Instance.OnSupplyChanged -= HandleSupplyChange;
        }

        private void HandleSupplyChange(int amount, SupplySO supplyType)
        {
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