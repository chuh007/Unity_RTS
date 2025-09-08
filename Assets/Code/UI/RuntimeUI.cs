using System;
using System.Collections.Generic;
using System.Linq;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Players;
using Code.UI.Containers;
using Code.Units;
using UnityEngine;

namespace Code.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private CommandUI commandUI;
        [SerializeField] private GenerateUnitUI generateUnitUI;
        [SerializeField] private SupplyUI supplyUI;
        [SerializeField] private UserSupplies userSupplies; // TODO 의존주입

        private HashSet<AbstractCommandable> _selectedUnits = new HashSet<AbstractCommandable>(12);

        private void Awake()
        {
            Bus<UnitSelectEvent>.OnEvent += HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent += HandleUnitDeselect;
        }

        private void Start()
        {
            DisableAllUI();
            supplyUI.EnableFor(userSupplies);
        }

        private void OnDestroy()
        {
            Bus<UnitSelectEvent>.OnEvent -= HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent -= HandleUnitDeselect;
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

                    if (firstUnit is BaseBuilding building)
                    {
                        generateUnitUI.EnableFor(building);
                    }
                }
            }
        }

        private void DisableAllUI()
        {
            commandUI.Disable();
            generateUnitUI.Disable();
        }
    }
}