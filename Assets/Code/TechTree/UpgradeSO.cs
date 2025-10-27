using System;
using System.Reflection;
using Code.Units;
using Code.Units.Data;
using UnityEngine;

namespace Code.TechTree
{
    public abstract class UpgradeSO : UnlockableSO, IModifier
    {
        [field: SerializeField] public string PropertyPath { get; private set; }
        
        public abstract void Apply(AbstractUnitSO unit);

        protected T GetPropertyValue<T>(AbstractUnitSO unit, out object target, out PropertyInfo propertyInfo)
        {
            string[] attributes = PropertyPath.Split('/'); //슬래시를 기준으로 나눠서 배열로 가져온다.
            
            Type type = unit.GetType();
            target = unit;

            //재귀적으로 내려가면서 타입을 찾는다.
            for (int i = 0; i < attributes.Length - 1; i++)
            {
                propertyInfo = type.GetProperty(attributes[i]);

                if (propertyInfo == null)
                {
                    Debug.LogError($"Unable to apply modifier {Name} to attribute " +
                                   $"{PropertyPath} because it does not contain a property named {unit.Name}.{attributes[i]}");
                    throw new InvalidPathException(attributes[i]);
                }
                
                target = propertyInfo.GetValue(target); //프로퍼티를 뽑아서 다시 타겟으로 가져온다.
                type = target.GetType(); //재귀적으로 계속 돌아간다.
            }

            propertyInfo = type.GetProperty(attributes[^1]); //마지막 원소가 진짜 원하는 프로퍼티임.
            if (propertyInfo == null)
            {
                Debug.LogError($"Unable to apply modifier {Name} to attribute {PropertyPath} " +
                               $"because it does not exist on {unit.Name}.{attributes[^1]}");
                throw new InvalidPathException(attributes[^1]);
            }

            T returnValue = default;
            try
            {
                returnValue = (T)propertyInfo.GetValue(target);
            }
            catch (InvalidCastException)
            {
                Debug.LogError($"Expected {PropertyPath} to be an {typeof(T).FullName}, but it wasn't");
            }
            
            return returnValue;
        }
    }
}