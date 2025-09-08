using System;
using UnityEngine;

namespace Code.UI.Components
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private RectTransform mask;
        private RectTransform maskParentRectTrm;
        [SerializeField, Range(0, 1f)] private float progress;
        [SerializeField] private Vector2 padding = new Vector2(9, 8);

        private void Awake()
        {
            Debug.Assert(mask != null, $"Progress bar {name} is missing a mask");
            maskParentRectTrm = mask.parent.GetComponent<RectTransform>();
        }
        
        public void SetProgress(float progress)
        {
            Vector2 parentSize = maskParentRectTrm.sizeDelta;
            Vector2 targetSize = parentSize - padding * 2;

            targetSize.x *= Mathf.Clamp01(progress);

            mask.offsetMin = padding;
            mask.offsetMax = new Vector2(padding.x + targetSize.x - parentSize.x, -padding.y);
            //upper right anchor 기준 upper right corner위치.
        }
    }
}