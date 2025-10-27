using Unity.Behavior;

namespace Code.Units.BT
{
    [BlackboardEnum]
    public enum ConstructionEventType
    {
        ArrivedAt,
        Begin,
        Cancel, //사망 등으로 인해 취소되었어
        Abort, //재개하러 갔는데 다른애가 이미 재개를 했어. 그래서 취소
        Completed,
    }
}