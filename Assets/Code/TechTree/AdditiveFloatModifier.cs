using System.Reflection;
using Code.Units.Data;
using UnityEngine;

namespace Code.TechTree
{
    [CreateAssetMenu(fileName = "", menuName = "Tech/Additive float", order = 151)]
    public class AdditiveFloatModifier : UpgradeSO
    {
        [field: SerializeField] public float Amount { get; private set; }
        
        public override void Apply(AbstractUnitSO unit)
        {
            try
            {
                float currentValue = GetPropertyValue<float>(unit, out object target, out PropertyInfo propertyInfo);
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