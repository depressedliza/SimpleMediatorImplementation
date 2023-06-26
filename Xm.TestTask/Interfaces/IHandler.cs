namespace Xm.TestTask.Interfaces;

public interface IHandler<in TData, TResponse>
{
    public Task<TResponse> Handle(TData data);
}
