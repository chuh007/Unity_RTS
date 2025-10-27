using Code.Units;
using Code.Units.Data;

namespace Code.TechTree
{
    public interface IModifier
    {
        string PropertyPath { get; }
        void Apply(AbstractUnitSO unit);
    }
}