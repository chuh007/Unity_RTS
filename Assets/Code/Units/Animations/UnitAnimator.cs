using System;
using UnityEngine;

namespace Code.Units.Animations
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void Awake()
        {
            Debug.Assert(animator != null, $"Animator is not assigned in {gameObject.name}");
        }

        public void SetParameter(ParameterSO parameter, bool value)
            => animator.SetBool(parameter.HashValue, value);
        public void SetParameter(ParameterSO parameter, float value)
            => animator.SetFloat(parameter.HashValue, value);
        public void SetParameter(ParameterSO parameter, int value)
            => animator.SetInteger(parameter.HashValue, value);
        public void SetParameter(ParameterSO parameter)
            => animator.SetTrigger(parameter.HashValue);
    }
}