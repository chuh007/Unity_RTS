using System;
using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Cheats
{
    public class ResourceCheats : MonoBehaviour
    {
        [SerializeField] private SupplySO mineralSO;
        [SerializeField] private SupplySO gasSO;

        private void Update()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Bus<SupplyEvent>.Raise(new SupplyEvent(1000, mineralSO));
            }

            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                Bus<SupplyEvent>.Raise(new SupplyEvent(1000, gasSO));
            }
        }
    }
}