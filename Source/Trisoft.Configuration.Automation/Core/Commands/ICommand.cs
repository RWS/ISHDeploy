using System;

namespace Trisoft.Configuration.Automation.Core.Commands
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<T>
    {
        event EventHandler<T> GetResult;

        void Execute();
    }
}
