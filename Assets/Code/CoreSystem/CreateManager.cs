using Code.GameEvents;
using Code.Units.Combat;
using GondrLib.Effects;
using ObjectPool.RunTime;
using Reflex.Attributes;
using UnityEngine;

namespace Code.CoreSystem
{
    public class CreateManager : MonoBehaviour
    {
        [Inject] private PoolManagerMono _poolManager;

        private void Awake()
        {
            Bus<ProjectileLaunchEvent>.RegisterForAll( HandleCreateProjectile);
            Bus<PoolEffectPlayEvent>.RegisterForAll( HandlePoolEffectPlay);
        }

        private void OnDestroy()
        {
            Bus<ProjectileLaunchEvent>.UnRegisterForAll( HandleCreateProjectile);
            Bus<PoolEffectPlayEvent>.UnRegisterForAll( HandlePoolEffectPlay);
        }

        private void HandlePoolEffectPlay(PoolEffectPlayEvent evt)
        {
            PoolingEffect poolingEffect = _poolManager.Pop<PoolingEffect>(evt.Item);
            poolingEffect.PlayVFX(evt.Position, evt.Rotation);
        }

        private void HandleCreateProjectile(ProjectileLaunchEvent evt)
        {
            Projectile projectile = _poolManager.Pop<Projectile>(evt.Item);
            projectile.LaunchProjectile(evt.StartPosition, evt.EndPosition, evt.Target, evt.Speed, evt.Damage);
        }
    }
}