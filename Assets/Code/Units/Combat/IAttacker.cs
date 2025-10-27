using UnityEngine;

namespace Code.Units.Combat
{
    public interface IAttacker
    {
        DamageableSensor Sensor { get; }
        Transform Transform { get; }

        void Attack(IDamageable damageable);
        void Attack(Vector3 location);
    }
}