namespace Trisoft.Configuration.Automation.Core
{
    public interface ICommandWithResult<T>
    {
        T Execute();
    }
}
