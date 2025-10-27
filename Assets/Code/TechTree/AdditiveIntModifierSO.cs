using System;
using System.Reflection;
using Code.Units.Data;
using UnityEngine;

namespace Code.TechTree
{
    [CreateAssetMenu(fileName = "Add integer", menuName = "Tech/ Additive int", order = 150)]
    public class AdditiveIntModifierSO : UpgradeSO
    {
        [field: SerializeField] public int Amount { get; private set; }

        public override void Apply(AbstractUnitSO unit)
        {
            try
            {
                int currentValue = GetPropertyValue<int>(unit, out object target, out PropertyInfo propertyInfo);
                currentValue += Amount;
                propertyInfo.SetValue(unit, currentValue);
                Debug.Log($"Update value : {currentValue}");
            }
            catch (InvalidPathException)
            {
                
            }
        }
    }
}