using Code.Units;
using TMPro;
using UnityEngine;

namespace Code.UI.Containers
{
    public class SingleUnitSelectUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private TextMeshProUGUI nameText;


        public void EnableFor(AbstractCommandable commandable)
        {
            gameObject.SetActive(true);
            nameText.SetText(commandable.UnitSo.Name);
        }

        public void Disable()
        {
            gameObject.SetActive(false);   
        }
    }
}