using System.Collections;
using Code.UI.Components;
using Code.Units;
using Code.Units.Data;
using TMPro;
using UnityEngine;

namespace Code.UI.Containers
{
    public class GenerateUnitUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private ProgressBarUI progressBar;
        [SerializeField] private BuildingQueueButtonUI[] queueButtons;

        private BaseBuilding _currentBuilding;
        private Coroutine _generateCoroutine;
        
        public void EnableFor(BaseBuilding item)
        {
            if (_currentBuilding != null)
            {
                _currentBuilding.OnQueueUpdated -= HandleQueueUpdate;
            }
            gameObject.SetActive(true);
            _currentBuilding = item;
            _currentBuilding.OnQueueUpdated += HandleQueueUpdate;
            buildingNameText.SetText(_currentBuilding.BuildingSo.Name);
            SetupQueueButtons();
            _generateCoroutine = StartCoroutine(UpdateUnitProgress());
        }
        
        public void Disable()
        {
            if(_currentBuilding != null)
                _currentBuilding.OnQueueUpdated -= HandleQueueUpdate;
            
            gameObject.SetActive(false);
            _currentBuilding = null;
            StopAllCoroutines();
            _generateCoroutine = null;
        }
        
        private void HandleQueueUpdate(UnitSO[] unitsInQueue)
        {
            if (unitsInQueue.Length == 1 && _generateCoroutine == null)
            {
                _generateCoroutine = StartCoroutine(UpdateUnitProgress());
            }

            if (_currentBuilding != null)
            {
                SetupQueueButtons();
            }
        }

        private void SetupQueueButtons()
        {
            int i = 0;
            for (; i < _currentBuilding.QueueSize; i++)
            {
                int idx = i;
                queueButtons[i].EnableFor(_currentBuilding.Queue[i], 
                    () => _currentBuilding.CancelGenerate(idx));
            }

            for (; i < queueButtons.Length; i++)
            {
                queueButtons[i].Disable();
            }
        }
        
        IEnumerator UpdateUnitProgress()
        {
            while (_currentBuilding != null && _currentBuilding.QueueSize > 0)
            {
                float startTime = _currentBuilding.CurrentQueueStartTime;
                float progress = Mathf.Clamp01(
                    (Time.time - startTime) / _currentBuilding.SOBeingBuilt.BuildTime);
                progressBar.SetProgress(progress);
                yield return null;
            }
            _generateCoroutine = null;
        }
    }
}