using System;
using UnityEngine;

namespace Code.Units.Animations
{
    [CreateAssetMenu(fileName = "Animator param", menuName = "Units/Parameter", order = 0)]
    public class ParameterSO : ScriptableObject
    {
        [field:SerializeField] public string ParamName { get; private set; }
        [field:SerializeField] public int HashValue { get; private set; }

        private void OnValidate()
        {
            HashValue = Animator.StringToHash(ParamName);
        }
    }
}