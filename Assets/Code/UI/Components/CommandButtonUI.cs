using System;
using Code.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UI.Components
{
    public class CommandButtonUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        private void Awake()
        {
            Debug.Assert(icon != null && button != null, $"Icon image component is not assigned in {gameObject.name}");
        }

        public void EnableFor(BaseCommandSO command, UnityAction onClick)
        {
            SetIcon(command.Icon);
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }

        public void Disable()
        {
            SetIcon(null);
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
        
        private void SetIcon(Sprite sprite)
        {
            if (sprite == null)
            {
                icon.enabled = false;
            }
            else
            {
                icon.sprite = sprite;
                icon.enabled = true;
            }
        }
    }
}