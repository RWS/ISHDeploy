using System.Collections.Generic;

namespace Trisoft.Configuration.Automation.Core
{

    public class CommandInvoker : ICommandInvoker, IRestorable
    {
        public readonly ILogger Logger;
        public readonly string ActivityDescription;
        private readonly List<IExecutable> _commands;


        public CommandInvoker(ILogger logger, string activityDescription)
        {
            Logger = logger;
            ActivityDescription = activityDescription;
            _commands = new List<IExecutable>();
        }

        public void AddCommand(IExecutable command)
        {
            _commands.Add(command);
        }

        public virtual void Invoke()
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                var command = _commands[i];

                command.Execute();

                Logger.WriteProgress(ActivityDescription, $"Executed {i}/{_commands.Count}");
            }
        }

        public void Backup()
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                var command = _commands[i];

                var restorable = command as IRestorable;
                restorable?.Backup();

                Logger.WriteProgress(ActivityDescription, $"Executed {i}/{_commands.Count}");
            }
        }

        public virtual void Rollback()
        {
            for (int i = _commands.Count - 1; i < _commands.Count; i--)
            {
                var command = _commands[i];

                var restorable = command as IRestorable;
                restorable?.Rollback();

                Logger.WriteProgress(ActivityDescription, $"Restored {i}/{_commands.Count}");
            }
        }

        public void Clear()
        {
            _commands.Clear();
        }
    }
}
