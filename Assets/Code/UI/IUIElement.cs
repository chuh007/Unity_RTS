namespace Code.UI
{
    public interface IUIElement<T>
    {
        void EnableFor(T item);
        void Disable();
    }
    
    public interface IUIElement<T1, T2>
    {
        void EnableFor(T1 item, T2 callback);
        void Disable();
    }
    
    //이건 지금은 안쓴다.
    public interface IUIElement<T1, T2, T3>
    {
        void EnableFor(T1 item, T2 context, T3 callback);
        void Disable();
    }
}