namespace Trisoft.Configuration.Automation.Core.Commands
{
    public interface ICommandWithResult<out T>
    {
        T Execute();
    }
}
