using Unity.Behavior;

namespace Code.Units.BT
{
    [BlackboardEnum]
    public enum ConstructionEventType
    {
        ArrivedAt,
        Begin,
        Cancel, // 사망 등으로 인한 취소
        About, // 재개하러 갔는데 다른 애가 이미 재개함
        Completed,
    }
}