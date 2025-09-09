using System;
using UnityEngine;

namespace Code.Units.Buildings
{
    public class ConstructionDummy : MonoBehaviour
    {
        [SerializeField] private GameObject[] constructionStage;
        [SerializeField] private GameObject ghostVisual;

        private int _currentStateIndex = 0;

        #region Debug region

        [SerializeField] private bool isDebugging = false;
        [SerializeField] private float constructTime = 5f;
        private float _debugTime = 0f;
        
        [ContextMenu("reset debug time")]
        private void ResetDebugTime()
        {
            ChangeConstructionStage(0);
            _debugTime = 0f;
        }

        private void Update()
        {
            if (isDebugging)
            {
                _debugTime += Time.deltaTime;
                UpdateConstructionProgress(_debugTime / constructTime);
            }
        }

        public void UpdateConstructionProgress(float progress)
        {
            if (progress < 0 || progress > 1 || _currentStateIndex >= constructionStage.Length)
                return;

            if (_currentStateIndex == 0 && progress > 0.45f)
                ChangeConstructionStage(1);

            if (_currentStateIndex == 1 && progress > 0.9f)
                ChangeConstructionStage(2);
        }

        #endregion

        public void ChangeConstructionStage(int index)
        {
            if (index < 0 || index >= constructionStage.Length)
                return;
            
            constructionStage[_currentStateIndex].SetActive(false);
            _currentStateIndex = index;
            constructionStage[_currentStateIndex].SetActive(true);
        }

        public void SetGhostVisual(bool isActive)
        {
            constructionStage[_currentStateIndex].SetActive(!isActive);
            ghostVisual.SetActive(isActive);
        }
    }
}