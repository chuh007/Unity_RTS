using Code.Environments;
using Code.Players;
using TMPro;
using UnityEngine;

namespace Code.UI.Containers
{
    public class SupplyUI : MonoBehaviour, IUIElement<UserSupplies>
    {
        [SerializeField] private SupplySO mineralSO;
        [SerializeField] private TextMeshProUGUI mineralText;
        [SerializeField] private SupplySO gasSO;
        [SerializeField] private TextMeshProUGUI gasText;
        
        private UserSupplies _userSupplies;
        
        public void EnableFor(UserSupplies item)
        {
            if (_userSupplies != null)
            {
                _userSupplies.OnSupplyChanged -= UpdateSupplyUI;
            }
            _userSupplies = item;
            _userSupplies.OnSupplyChanged += UpdateSupplyUI;
            
            UpdateSupplyUI(_userSupplies.Minerals, mineralSO);
            UpdateSupplyUI(_userSupplies.Gas, gasSO);
            
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            _userSupplies.OnSupplyChanged -= UpdateSupplyUI;
            _userSupplies = null;
        }

        private void UpdateSupplyUI(int amount, SupplySO supplytype)
        {
            if(supplytype.Equals(mineralSO))
                mineralText.SetText(amount.ToString());
            if (supplytype.Equals(gasSO))
                gasText.SetText(amount.ToString());
        }
    }
}