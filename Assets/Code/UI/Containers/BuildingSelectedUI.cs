using Code.Units;
using Code.Units.Buildings;
using Code.Units.Data;
using UnityEngine;

namespace Code.UI.Containers
{
    public class BuildingSelectedUI : MonoBehaviour, IUIElement<IBuilding>
    {
        [SerializeField] private GenerateUnitUI generateUnitUI;
        [SerializeField] private UnderConstructionUI underConstructionUI;
        [SerializeField] private SingleUnitSelectUI singleUnitSelectUI;

        private BaseBuilding _selectedBuilding;
        
        public void EnableFor(IBuilding building)
        {
            DisableAllUI();
            if (building is BaseBuilding baseBuilding)
            {
                _selectedBuilding = baseBuilding;
                _selectedBuilding.OnQueueUpdated -= HandleOnQueueUpdated;
                _selectedBuilding.OnQueueUpdated += HandleOnQueueUpdated;
                
                HandleOnQueueUpdated();
            }else if (building is ConstructionDummy dummy)
            {
                underConstructionUI.EnableFor(dummy);
            }
        }

        private void HandleOnQueueUpdated(UnlockableSO[] _ = null)
        {
            if (_selectedBuilding == null) return;

            if (_selectedBuilding.QueueSize > 0)
            {
                generateUnitUI.EnableFor(_selectedBuilding);
                singleUnitSelectUI.Disable();
            }
            else
            {
                singleUnitSelectUI.EnableFor(_selectedBuilding);
                generateUnitUI.Disable();
            }
        }

        public void Disable()
        {
            if (_selectedBuilding != null)
            {
                _selectedBuilding.OnQueueUpdated -= HandleOnQueueUpdated;
                _selectedBuilding = null;
            }
            DisableAllUI();
        }
        
        private void DisableAllUI()
        {
            generateUnitUI.Disable();
            underConstructionUI.Disable();
            singleUnitSelectUI.Disable();
        }
    }
}