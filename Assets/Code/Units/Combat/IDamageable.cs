using UnityEngine;

namespace Code.Units.Combat
{
    public interface IDamageable
    {
        int MaxHealth { get; }
        int CurrentHealth { get; }
        Transform Transform { get; }
        Owner Owner { get; }

        void TakeDamage(int damage, Vector3 position, Vector3 normal);
        void Die();
    }
}