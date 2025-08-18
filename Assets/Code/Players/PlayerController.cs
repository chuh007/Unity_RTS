using System;
using Code.CoreSystem;
using Code.Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Players
{
    public enum ButtonState
    {
        None, Pressed, Released, Held
    }
    
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputReaderSO inputReader;
        [SerializeField] private LayerMask selectableUnitLayer;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField] private RectTransform selectionBox;
        
        [Space]
        [Header("Camera settings")]
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private Rigidbody cameraTarget;
        
        private ISelectable _selectedUnit;
        private ButtonState _leftButtonState = ButtonState.None;
        private Vector2 _startingMousePosition;
        
        private void Awake()
        {
            inputReader.OnMouseLeftButton += HandleMouseLeftButton;
            inputReader.OnMouseRightButton += HandleMouseRightButton;
        }

        private void OnDestroy()
        {
            inputReader.OnMouseLeftButton -= HandleMouseLeftButton;
            inputReader.OnMouseRightButton -= HandleMouseRightButton;
        }
        
        private void Update()
        {
            UpdateLeftButtonState();
            HandlePanning();
            HandleDragSelect();
        }

        private void UpdateLeftButtonState()
        {
            if(Mouse.current.leftButton.wasPressedThisFrame)
                _leftButtonState = ButtonState.Pressed;
            else if (Mouse.current.leftButton.isPressed
                     && !Mouse.current.leftButton.wasPressedThisFrame)
                _leftButtonState = ButtonState.Held;
            else if(Mouse.current.leftButton.wasReleasedThisFrame)
                _leftButtonState = ButtonState.Released;
            else 
                _leftButtonState = ButtonState.None;
        }   
        
        private void HandlePanning()
        {
            Vector2 movement = inputReader.KeyboardMovement * cameraConfig.KeyboardPanSpeed;
            movement += GetMouseMoveAmount();
            cameraTarget.linearVelocity = new Vector3(movement.x, 0, movement.y);
        }
        
        private void HandleDragSelect()
        {
            if (selectionBox == null) return;

            switch (_leftButtonState)
            {
                case ButtonState.Pressed:
                {
                    selectionBox.sizeDelta = Vector2.zero;
                    selectionBox.gameObject.SetActive(true);
                    _startingMousePosition = inputReader.MousePosition;
                    break;
                }
                case ButtonState.Held:
                {
                    Vector2 mousePosition = inputReader.MousePosition;
                    Bounds bound = ResizeSelectionBox(mousePosition);
                    break;
                }
                case ButtonState.Released:
                {
                    selectionBox.gameObject.SetActive(false);
                    break;
                }
            }
        }
        
        private Vector2 GetMouseMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;
            
            if(cameraConfig.EnableEdgePan == false) return moveAmount;

            Vector2 mousePosition = inputReader.MousePosition;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (mousePosition.x < cameraConfig.EdgePanSize)
            {
                moveAmount.x -= cameraConfig.MousePanSpeed;
            }else if (mousePosition.x > screenSize.x - cameraConfig.EdgePanSize)
            {
                moveAmount.x += cameraConfig.MousePanSpeed;
            }else if (mousePosition.y < cameraConfig.EdgePanSize)
            {
                moveAmount.y -= cameraConfig.MousePanSpeed;
            }else if (mousePosition.y > screenSize.y - cameraConfig.EdgePanSize)
            {
                moveAmount.y += cameraConfig.MousePanSpeed;
            }
            return moveAmount;
        }
        
        private void HandleMouseLeftButton(bool isPressed)
        {
            if (isPressed)
            {
                if (inputReader.GetMousePosition(out RaycastHit hit, selectableUnitLayer)
                    && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    if (_selectedUnit != null)
                    {
                        _selectedUnit.Deselect();
                    }
                    
                    _selectedUnit = selectable;
                    selectable.Select();
                }
                else if (_selectedUnit != null)
                {
                    _selectedUnit.Deselect();
                    _selectedUnit = null;
                }
            }
        }
        
        private void HandleMouseRightButton(bool isPressed)
        {
            if(_selectedUnit == null || _selectedUnit is not IMoveable moveable) return;

            if (isPressed)
            {
                if (inputReader.GetMousePosition(out RaycastHit hit, floorLayer | selectableUnitLayer))
                {
                    moveable.MoveTo(hit.point);
                }
            }
        }
        
        private Bounds ResizeSelectionBox(Vector2 mousePosition)
        {
            Vector2 delta = mousePosition - _startingMousePosition;
            selectionBox.anchoredPosition = _startingMousePosition + delta * 0.5f;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
            
            return new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        }

    }
}