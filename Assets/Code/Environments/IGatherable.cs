namespace Code.Environments
{
    public interface IGatherable
    {
        SupplySO SupplyData { get; }
        int Amount { get; } //현재 남은 갯수
        bool IsBusy { get; } //다른 유닛이 캐고 있으면 못캔다.

        bool BeginGather(); //수집시작.
        int EndGather(); //수집 종료
        void AbortGather(); //수집 취소
    }
}