using System.Linq;
using Code.Players;
using Code.Units;
using Code.Units.Buildings;
using Code.Units.Data;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Construct building", menuName = "Units/Commands/Construct building", order = 0)]
    public class ConstructBuildingCommandSO : BaseCommandSO, IConstructionCommand
    {
        [field: SerializeField] public BuildingSO BuildingData { get; private set; }
        [field: SerializeField] public GameObject GhostPrefab { get; private set; }
        [field: SerializeField] public ConstructRestrictionSO[] Restrictions { get; private set; }
        
        public override bool CanHandle(CommandContext context)
        {
            if (context.Commandable is not IBuildingConstructor constructor || constructor.IsBuilding) return false;
            
            if (context.Hit.collider != null && context.MouseButton == MouseButton.Right)
            {
                //지어지다만 건물을 클릭한건지 체크
                return context.Hit.collider.TryGetComponent(out ConstructionDummy dummy)
                       && dummy.BuildingSo == BuildingData
                       && dummy.ProgressData.State == BuildingState.Paused;
            }
            
            return UserSupplies.Instance != null
                && UserSupplies.Instance.HasEnoughSupplies(BuildingData.Cost)
                && AllRestrictionPass(context.Hit.point);
        }

        public bool AllRestrictionPass(Vector3 hitPoint)
            => Restrictions.Length == 0 || Restrictions.All(res => res.CanPlace(hitPoint));

        public override void Handle(CommandContext context)
        {
            IBuildingConstructor constructor = context.Commandable as IBuildingConstructor;

            if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out ConstructionDummy dummy))
            {
                //건설 재개를 하거나.
                constructor.ResumeConstruction(dummy);
            }
            else if(UserSupplies.Instance != null
                    && UserSupplies.Instance.HasEnoughSupplies(BuildingData.Cost)
                    && AllRestrictionPass(context.Hit.point))
            {       
                //신규 건설을 하거나.
                constructor.ConstructBuilding(BuildingData, context.Hit.point);
            }
        }
        
        public override bool IsLocked(CommandContext context)
            => UserSupplies.Instance != null
               && !UserSupplies.Instance.HasEnoughSupplies(BuildingData.Cost);
    }
}