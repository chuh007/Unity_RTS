using Code.Environments;
using Code.Players;
using Code.Units;
using TMPro;
using UnityEngine;
//여기 희섭이가 바꾸라고 지시했어. 안바꾸면 죽여버린다고함.
namespace Code.UI.Containers
{
    public class SupplyUI : MonoBehaviour, IUIElement<UserSupplies>
    {
        [SerializeField] private SupplySO mineralSO;
        [SerializeField] private TextMeshProUGUI mineralText;
        [SerializeField] private SupplySO gasSO;
        [SerializeField] private TextMeshProUGUI gasText;
        [SerializeField] private Owner uiOwner = Owner.Player;
        
        private UserSupplies _userSupplies;
        
        public void EnableFor(UserSupplies item)
        {
            if (_userSupplies != null)
            {
                _userSupplies.OnSupplyChanged -= UpdateSupplyUI;    
            }
            _userSupplies = item;
            _userSupplies.OnSupplyChanged += UpdateSupplyUI;
            
            UpdateSupplyUI(uiOwner, _userSupplies.Minerals[uiOwner], mineralSO);
            UpdateSupplyUI(uiOwner, _userSupplies.Gas[uiOwner], gasSO);
            
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            _userSupplies.OnSupplyChanged -= UpdateSupplyUI;
            _userSupplies = null;
        }

        private void UpdateSupplyUI(Owner owner, int amount, SupplySO supplyType)
        {
            if (owner != uiOwner) return;
            
            if(supplyType.Equals(mineralSO))
                mineralText.SetText(amount.ToString());
            if(supplyType.Equals(gasSO))
                gasText.SetText(amount.ToString());
        }
    }
}