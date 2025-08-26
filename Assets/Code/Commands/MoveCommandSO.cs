using Code.Units;
using UnityEngine;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Move command", menuName = "Units/Commands/Move", order = 100)]
    public class MoveCommandSO : BaseCommandSO
    {
        [SerializeField] private float radiusMultiplier = 2.5f;
        private int _unitsOnOrbit = 0;
        private int _maxUnitsOnOrbit = 1;
        private float _circleRadius = 0; //궤도의 크기
        private float _radialOffset = 0; //링을 돌면서 떨어져있는 정도
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is AbstractUnit;
        }

        public override void Handle(CommandContext context)
        {
            AbstractUnit unit = context.Commandable as AbstractUnit;

            if (context.UnitIndex == 0)
            {
                _unitsOnOrbit = 0;
                _maxUnitsOnOrbit = 1;
                _circleRadius = 0;
                _radialOffset = 0;
            }
            
            
            Vector3 targetPosition = new Vector3(
                context.Hit.point.x + _circleRadius * Mathf.Cos(_radialOffset * _unitsOnOrbit),
                context.Hit.point.y,
                context.Hit.point.z + _circleRadius * Mathf.Sin(_radialOffset * _unitsOnOrbit));
            
            unit.MoveTo(targetPosition);
            _unitsOnOrbit++; //이 궤도상에 있는 유닛수를 하나 증가.
            
            if (_unitsOnOrbit >= _maxUnitsOnOrbit)
            {
                _unitsOnOrbit = 0;
                _circleRadius += unit.AgentRadius * radiusMultiplier;
                _maxUnitsOnOrbit = Mathf.FloorToInt(2 * Mathf.PI * _circleRadius / (unit.AgentRadius * 2));
                _radialOffset = 2 * Mathf.PI / _maxUnitsOnOrbit;
            }
        }
    }
}