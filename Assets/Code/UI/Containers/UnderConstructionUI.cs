using System.Collections;
using Code.UI.Components;
using Code.Units.Buildings;
using TMPro;
using UnityEngine;

namespace Code.UI.Containers
{
    public class UnderConstructionUI : MonoBehaviour, IUIElement<ConstructionDummy>
    {
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private ProgressBarUI progressBar;
        
        public void EnableFor(ConstructionDummy dummy)
        {
            gameObject.SetActive(true);
            unitNameText.SetText(dummy.UnitSo.Name);
            StartCoroutine(AnimateBuildingProgress(dummy));
        }
        
        public void Disable()
        {
            gameObject.SetActive(false);
            StopAllCoroutines();
        }
        
        private IEnumerator AnimateBuildingProgress(ConstructionDummy dummy)
        {
            while (enabled && dummy.ProgressData.Progress < 1)
            {
                if (dummy.ProgressData.State != BuildingState.Constructing)
                {
                    yield return null;
                    continue;
                }
                
                float startTime = dummy.ProgressData.StartTime;
                // float endTime = startTime + dummy.BuildingSo.BuildTime;
                
                progressBar.SetProgress(Mathf.Clamp01((Time.time - startTime) / dummy.BuildingSo.BuildTime));
                yield return null;
            }
        }
    }
}