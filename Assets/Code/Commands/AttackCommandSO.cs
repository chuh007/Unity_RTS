using Code.Units.Combat;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Attack command", menuName = "Units/Commands/Attack", order = 99)]
    public class AttackCommandSO : BaseCommandSO
    {
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is IAttacker
                   && context.Hit.collider != null;
        }

        public override void Handle(CommandContext context)
        {
            IAttacker attacker = context.Commandable as IAttacker;
            Debug.Assert(attacker != null, $"Attacker is null, check Can handle first");
            
            if(context.Hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                attacker.Attack(damageable);
            }
            else
            {
                attacker.Attack(context.Hit.point);
            }
            
        }

        public override bool IsLocked(CommandContext context) => false;
    }
}