using System;
using System.Text;
using Code.Commands;
using Code.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI.Components
{
    public class CommandButtonUI : MonoBehaviour, IUIElement<BaseCommandSO, UnityAction>
    , IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Tooltip tooltip;
        [SerializeField] private Color disabledColor;
        [SerializeField] private Owner uiOwner = Owner.Player;
        
        private RectTransform _rectTrm;

        private static readonly string MINERAL_FORMAT = "<color=#00ACFF>{0}</color> Minerals.";
        private static readonly string GAS_FORMAT = "<color=#3BEA60>{0}</color> Gas.";
        private static readonly string DEPENDENCY_FORMAT_NO_COMMA = "<color=#AC0000>{0}</color>.";
        private static readonly string DEPENDENCY_FORMAT_COMMA = "<color=#AC0000>{0}</color>,";
        
        public bool IsActive { get; private set; }
        
        private void Awake()
        {
            Debug.Assert(icon != null && button != null, $"Image or Button component is not assigned in {gameObject.name}");
            _rectTrm = GetComponent<RectTransform>();
        }

        public void EnableFor(BaseCommandSO command, UnityAction onClick)
        {
            SetIcon(command.Icon);
            button.interactable = !command.IsLocked(new CommandContext(uiOwner));
            icon.color = button.interactable ? Color.white : disabledColor;

            IsActive = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);

            if (tooltip != null)
            {
                tooltip.SetText(GetTooltipText(command));
            }
        }

        public void Disable()
        {
            SetIcon(null);
            IsActive = false;
            button.interactable = false;
            button.onClick.RemoveAllListeners();
            
            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(tooltip != null && IsActive)
                Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke();
            tooltip?.Hide();
        }

        private void ShowTooltip()
        {
            tooltip.Show();
            //툴팁 피봇이 우측으로 만들어져있으니까 0에서 절반만큼 우측 이동을 하면 정확하게 중앙에 자리잡는다.
            tooltip.RectTransform.position = new Vector2(
                _rectTrm.position.x + _rectTrm.rect.width * 0.5f,
                _rectTrm.position.y + _rectTrm.rect.height * 0.5f);
        }

        private string GetTooltipText(BaseCommandSO command)
        {
            StringBuilder tooltipBuilder = new StringBuilder();
            tooltipBuilder.Append($"{command.Name}\n");

            SupplyCostSO cost = null;
            if (command is BuildUnitCommandSO buildUnitCommand)
            {
                cost = buildUnitCommand.Unit.Cost;
            }else if (command is ConstructBuildingCommandSO constructBuildingCommand)
            {
                cost = constructBuildingCommand.BuildingData.Cost;
            }

            if (cost != null)
            {
                if (cost.Minerals > 0)
                    tooltipBuilder.Append(string.Format(MINERAL_FORMAT, cost.Minerals));
                if(cost.Gas > 0)
                    tooltipBuilder.Append(string.Format(GAS_FORMAT, cost.Gas));
            }

            return tooltipBuilder.ToString();
        }
    }
}