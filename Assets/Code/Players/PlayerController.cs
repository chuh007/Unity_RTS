using System;
using System.Collections.Generic;
using System.Linq;
using Code.CoreSystem;
using Code.GameEvents;
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
        
        // private ISelectable _selectedUnit;
        private HashSet<ISelectable> _selectedUnits = new HashSet<ISelectable>(12);
        private HashSet<AbstractUnit> _aliveUnits = new HashSet<AbstractUnit>(200);
        private HashSet<AbstractUnit> _addedUnits = new HashSet<AbstractUnit>(24);
        
        private ButtonState _leftButtonState = ButtonState.None;
        private Vector2 _startingMousePosition;
        
        private void Awake()
        {
            inputReader.OnMouseRightButton += HandleMouseRightButton;

            Bus<UnitSelectEvent>.OnEvent += HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent += HandleUnitDeselect;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawn;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        }
        
        private void OnDestroy()
        {
            inputReader.OnMouseRightButton -= HandleMouseRightButton;
            
            Bus<UnitSelectEvent>.OnEvent -= HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent -= HandleUnitDeselect;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawn;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
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
                case ButtonState.Pressed: HandleDragStart(); break;
                case ButtonState.Held: HandleMouseDrag(); break;
                case ButtonState.Released: HandleDragEnd(); break;
            }
        }
        
        private void HandleDragStart()
        {
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(true);
            _startingMousePosition = inputReader.MousePosition;
        }
        
        private void HandleMouseDrag()
        {
            Vector2 mousePosition = inputReader.MousePosition;
            Bounds bound = ResizeSelectionBox(mousePosition);
                    
            foreach (AbstractUnit unit in _aliveUnits)
            {
                if (unit.gameObject.activeInHierarchy == false) continue;

                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);
                if (bound.Contains(unitScreenPosition))
                {
                    _addedUnits.Add(unit);
                }
                else
                {
                    _addedUnits.Remove(unit);
                }
            }
        }
        
        private void HandleDragEnd()
        {
            if(Keyboard.current.leftShiftKey.isPressed == false)
                DeselectAllUnits();
            HandleMouseLeftButton();
            foreach (AbstractUnit unit in _addedUnits)
            {
                unit.Select();
            }
            selectionBox.gameObject.SetActive(false);
        }

        private void DeselectAllUnits()
        {
            foreach (ISelectable selectable in _selectedUnits.ToArray())
            {
                selectable.Deselect();
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
        
        private void HandleMouseLeftButton()
        {
            if (inputReader.GetMousePosition(out RaycastHit hit, selectableUnitLayer)
                && hit.collider.TryGetComponent(out ISelectable selectable)
                && _addedUnits.Count == 0)
            {
                selectable.Select();
            }
        }
        
        private void HandleMouseRightButton(bool isPressed)
        {
            if(_selectedUnits.Count == 0) return;
            
            if (!isPressed && inputReader.GetMousePosition(out RaycastHit hit, floorLayer | selectableUnitLayer))
            {
                List<AbstractUnit> unitToMove = _selectedUnits.OfType<AbstractUnit>().ToList();

                int unitOnOrbit = 0;
                int maxUnitsOnOrbit = 1;
                float circleRadius = 0; // 궤도의 크기
                float radialOffset = 0; // 링을 돌면서 떨여져있는 정도

                foreach (AbstractUnit unit in unitToMove)
                {
                    Vector3 targetPosition = new Vector3(
                        hit.point.x + circleRadius * Mathf.Cos(radialOffset * unitOnOrbit),
                        hit.point.y,
                        hit.point.z + circleRadius * Mathf.Sin(radialOffset * unitOnOrbit));
                    
                    unit.MoveTo(targetPosition);
                    
                    unitOnOrbit++; // 이 궤도상에 잇는 유닛 수를 하나 증가
                    if (unitOnOrbit >= maxUnitsOnOrbit)
                    {
                        unitOnOrbit = 0;
                        circleRadius += unit.AgentRadius * 2.5f;
                        maxUnitsOnOrbit = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (unit.AgentRadius * 2));
                        radialOffset = 2 * Mathf.PI / maxUnitsOnOrbit;
                    }
                }
                
                // foreach (ISelectable selectable in _selectedUnits)
                // {
                //     if (selectable is IMoveable moveable)
                //         moveable.MoveTo(hit.point);
                // }
            }
        }
        
        private Bounds ResizeSelectionBox(Vector2 mousePosition)
        {
            Vector2 delta = mousePosition - _startingMousePosition;
            selectionBox.anchoredPosition = _startingMousePosition + delta * 0.5f;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
            
            return new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        }

        private void HandleUnitSelect(UnitSelectEvent evt)
            => _selectedUnits.Add(evt.Unit);
        private void HandleUnitDeselect(UnitDeselectEvent evt)
            => _selectedUnits.Remove(evt.Unit);
        private void HandleUnitSpawn(UnitSpawnEvent evt)
            => _aliveUnits.Add(evt.Unit);
        private void HandleUnitDeath(UnitDeathEvent evt)
            => _aliveUnits.Remove(evt.Unit);
    }
}