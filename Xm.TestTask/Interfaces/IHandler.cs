namespace Xm.TestTask.Interfaces;

public interface IHandler<TData, TResponse>
{
    public Task<TResponse> Handle(TData data);
}
