
namespace Commander
{
    public interface ICommandResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
    }
    public interface ICommandResult<out T> : ICommandResult
    {
        public T Data { get; }
    }
}
