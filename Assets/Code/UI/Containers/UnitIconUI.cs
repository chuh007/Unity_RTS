using Code.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Containers
{
    public class UnitIconUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI healthText;
        
        private AbstractCommandable _commandable;
        private const string HEALTH_FORMAT = "{0} / {1}";
        
        public void EnableFor(AbstractCommandable commandable)
        {
            gameObject.SetActive(true);
            healthText.SetText(string.Format(HEALTH_FORMAT, commandable.CurrentHealth, commandable.MaxHealth));
            icon.sprite = commandable.UnitSo.Icon;
            _commandable = commandable;

            _commandable.OnHealthUpdated -= HandleHealthUpdate;
            _commandable.OnHealthUpdated += HandleHealthUpdate;

        }

        private void HandleHealthUpdate(AbstractCommandable commandable, int lastHealth, int newHealth)
        {
            if(healthText == null) return;
            healthText.SetText(string.Format(HEALTH_FORMAT, newHealth, commandable.MaxHealth));
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            if (_commandable != null)
            {
                _commandable.OnHealthUpdated -= HandleHealthUpdate;
                _commandable = null;
            }
        }
    }
}