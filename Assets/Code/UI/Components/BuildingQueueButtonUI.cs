using System;
using Code.Units.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UI.Components
{
    public class BuildingQueueButtonUI : MonoBehaviour, IUIElement<UnitSO, UnityAction>
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        private void Awake()
        {
            Disable();
        }

        public void EnableFor(UnitSO item, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = true;
            button.onClick.AddListener(callback);
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = item.Icon;
        }

        public void Disable()
        {
            button.interactable = false;
            button.onClick.RemoveAllListeners();
            iconImage.gameObject.SetActive(false);
        }
    }
}