using System;
using System.Collections.Generic;
using System.Linq;
using Code.Commands;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units;
using Code.Units.Buildings;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Code.Players
{
    public enum ButtonState
    {
        None, Pressed, Released, Held
    }
    
    public class PlayerController : MonoBehaviour
    {
        [Header("Selection settings")]
        [SerializeField] private InputReaderSO inputReader;
        [SerializeField] private LayerMask selectableUnitLayer;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private RectTransform selectionBox;

        [Space]
        [Header("Camera settings")] 
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private Rigidbody cameraTarget;

        //private ISelectable _selectedUnit;
        private HashSet<ISelectable> _selectedUnits = new HashSet<ISelectable>(12);
        private HashSet<AbstractUnit> _aliveUnits = new HashSet<AbstractUnit>(200);
        private HashSet<AbstractUnit> _addedUnits = new HashSet<AbstractUnit>(24);
        
        private ButtonState _leftButtonState = ButtonState.None;
        private Vector2 _startingMousePosition;
        private BaseCommandSO _activeCommand;//현재 선택된 커맨드
        private bool _wasMouseDownOnUI = false;

        private GhostPlacement _ghostInstance;
        
        private void Awake()
        {
            inputReader.OnMouseRightButton += HandleMouseRightButton;

            Bus<UnitSelectEvent>.OnEvent += HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent += HandleUnitDeselect;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawn;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            Bus<CommandSelectEvent>.OnEvent += HandleCommandSelect;
        }

        private void OnDestroy()
        {
            inputReader.OnMouseRightButton -= HandleMouseRightButton;
            
            Bus<UnitSelectEvent>.OnEvent -= HandleUnitSelect;
            Bus<UnitDeselectEvent>.OnEvent -= HandleUnitDeselect;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawn;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<CommandSelectEvent>.OnEvent -= HandleCommandSelect;
        }

        private void Update()
        {
            UpdateLeftButtonState();
            HandlePanning();
            HandleDragSelect();
            HandleGhostInstance();
        }
        
        private void UpdateLeftButtonState()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _leftButtonState = ButtonState.Pressed;
            }
            else if (Mouse.current.leftButton.isPressed
                     && !Mouse.current.leftButton.wasPressedThisFrame)
            {
                _leftButtonState = ButtonState.Held;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _leftButtonState = ButtonState.Released;
            }
            else
            {
                _leftButtonState = ButtonState.None;
            }
        }

        private void HandlePanning()
        {
            Vector2 movement = inputReader.KeyboardMovement * cameraConfig.KeyboardPanSpeed;
            movement += GetMouseMoveAmount();
            cameraTarget.linearVelocity = new Vector3(movement.x, 0, movement.y);
        }

        private void HandleDragSelect()
        {
            if (selectionBox == null) return; //안전하게 예외처리

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
            _addedUnits.Clear(); //이번 드래그에 추가되는 유닛
            _wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject();
        }
        
        private void HandleMouseDrag()
        {
            if (_activeCommand != null || _wasMouseDownOnUI) return;
            
            Vector2 mousePosition = inputReader.MousePosition;
            Bounds bound = ResizeSelectionBox(mousePosition);

            foreach (AbstractUnit unit in _aliveUnits)
            {
                if(unit.gameObject.activeInHierarchy == false) continue;

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
            if(!_wasMouseDownOnUI && _activeCommand == null && Keyboard.current.shiftKey.isPressed == false)
                DeselectAllUnits();
            
            HandleMouseLeftButton();
            foreach (AbstractUnit unit in _addedUnits)
            {
                unit.Select();
            }
            selectionBox.gameObject.SetActive(false);
        }
        
        private void HandleGhostInstance()
        {
            if(_ghostInstance == null) return;
            
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Destroy(_ghostInstance.gameObject);
                _ghostInstance = null;
                _activeCommand = null;
                return;
            }

            if (inputReader.GetMousePosition(out RaycastHit hit, floorLayer))
            {
                _ghostInstance.transform.position = hit.point;

                if (_activeCommand is IConstructionCommand command)
                {
                    bool isPassRestiction = command.AllRestrictionPass(hit.point);
                    _ghostInstance.SetPassRestriction(isPassRestiction);
                }
            }
        }
        
        private void DeselectAllUnits()
        {
            foreach (ISelectable selectable in _selectedUnits.ToArray())
            {
                selectable.DeSelect();
            }
        }

        private void HandleMouseLeftButton()
        {
            if (_wasMouseDownOnUI) return;
            
            RaycastHit hit;
            
            if (_activeCommand == null
                && inputReader.GetMousePosition(out hit, selectableUnitLayer)
                && hit.collider.TryGetComponent(out ISelectable selectable)
                && _addedUnits.Count == 0) //드래그 마지막에 건물같은게 추가되지 않도록
            {
                selectable.Select();
            }else if (_activeCommand != null
                      && inputReader.GetMousePosition(out hit, floorLayer | interactableLayer))
            {
                ActivateCommand(hit);
            }
        }

        private void HandleMouseRightButton(bool isPressed)
        {
            if (_selectedUnits.Count == 0) return;

            if (!isPressed 
                && inputReader.GetMousePosition(out RaycastHit hit, floorLayer | interactableLayer))
            {
                List<AbstractUnit> selectedUnits = _selectedUnits.OfType<AbstractUnit>().ToList();

                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    CommandContext context = new CommandContext(selectedUnits[i], hit, i, MouseButton.Right);

                    foreach (BaseCommandSO command in GetAvailableCommands(selectedUnits[i]))
                    {
                        if (command.CanHandle(context))
                        {
                            command.Handle(context);
                            if (command.IsSingleUnitCommand) return;
                            break; //첫번째로 가용한 명령을 수행한다.
                        }
                    }
                }
                
            }
        }
        
        private void HandleUnitSelect(UnitSelectEvent evt)
            => _selectedUnits.Add(evt.Unit);
        private void HandleUnitDeselect(UnitDeselectEvent evt)
            => _selectedUnits.Remove(evt.Unit);
        private void HandleUnitSpawn(UnitSpawnEvent evt)
            => _aliveUnits.Add(evt.Unit);

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            _aliveUnits.Remove(evt.Unit);
            _selectedUnits.Remove(evt.Unit);
        }
        
        
        private void HandleCommandSelect(CommandSelectEvent evt)
        {
            DestroyGhostIfExist();
            _activeCommand = evt.Command;
            if (_activeCommand.RequireClickToActivate == false)
                ActivateCommand(new RaycastHit());
            else if (_activeCommand is IConstructionCommand buildCommand)
            {
                if (buildCommand.GhostPrefab != null)
                {
                    GameObject ghost = Instantiate(buildCommand.GhostPrefab);
                    _ghostInstance = ghost.GetComponent<GhostPlacement>();
                    Debug.Assert(_ghostInstance != null,
                        $"GhostPlacement script is missing on {buildCommand.GhostPrefab.name}");
                }
            }
        }

        private void DestroyGhostIfExist()
        {
            if (_ghostInstance != null)
            {
                Destroy(_ghostInstance.gameObject);
                _ghostInstance = null;
            }
        }

        private void ActivateCommand(RaycastHit hit)
        {
            if (_ghostInstance != null)
            {
                Destroy(_ghostInstance.gameObject);
                _ghostInstance = null;
            }
            
            List<AbstractCommandable> abstractUnits = _selectedUnits.OfType<AbstractCommandable>().ToList();
            
            for (int i = 0; i < abstractUnits.Count; i++)
            {
                CommandContext context = new CommandContext(abstractUnits[i], hit, i);

                if (_activeCommand.CanHandle(context))
                {
                    _activeCommand.Handle(context);
                    if (_activeCommand.IsSingleUnitCommand)
                        break;
                }
            }
            
            _activeCommand = null;
        }

        #region Utility

        private Vector2 GetMouseMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;

            if (cameraConfig.EnableEdgePan == false) return moveAmount;

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
        
        private Bounds ResizeSelectionBox(Vector2 mousePosition)
        {
            Vector2 delta = mousePosition - _startingMousePosition; //움직인 양.
            selectionBox.anchoredPosition = _startingMousePosition + delta * 0.5f;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

            return new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        }

        private List<BaseCommandSO> GetAvailableCommands(AbstractUnit targetUnit)
        {
            OverrideCommandsCommandSO[] overrideCommands
                = targetUnit.AvailableCommands.OfType<OverrideCommandsCommandSO>().ToArray();
            
            List<BaseCommandSO> allAvaliableCommands = new List<BaseCommandSO>();
            foreach (var overrideCommand in overrideCommands)
            {
                allAvaliableCommands.AddRange(
                    overrideCommand.Commands.Where(command => command is not OverrideCommandsCommandSO));
            }

            allAvaliableCommands.AddRange(
                targetUnit.AvailableCommands.Where(command => command is not OverrideCommandsCommandSO));
            
            return allAvaliableCommands;
        }
        
        #endregion

    }
}