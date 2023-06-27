namespace Xm.TestTask.Interfaces;

public interface IMediator
{ 
    public Task<TResponse> Dispatch<TResponse>(string action, byte[] data);
    public Task<object> Dispatch(string action, byte[] data);
}
