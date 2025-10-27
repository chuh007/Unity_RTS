using System;
using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using Code.Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Cheats
{
    public class ResourceCheats : MonoBehaviour
    {
        [SerializeField] private SupplySO mineralSO;
        [SerializeField] private SupplySO gasSO;
        [SerializeField] private Owner cheatOwner = Owner.Player;
        private void Update()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Bus<SupplyEvent>.Raise(cheatOwner, new SupplyEvent(cheatOwner, 1000, mineralSO));
            }
            
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                Bus<SupplyEvent>.Raise(cheatOwner, new SupplyEvent(cheatOwner, 1000, gasSO));
            }
        }
    }
}