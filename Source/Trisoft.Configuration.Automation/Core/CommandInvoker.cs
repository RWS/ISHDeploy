using System.Collections.Generic;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.Invokers
{

    public class CommandInvoker<T> : ICommandInvoker<T>
        where T : ICommand
    {
        public readonly ILogger Logger;
        public readonly bool IsBackupEnabled;
        public readonly string ActivityDescription;
        public readonly ISHProject ISHProject;
        private readonly List<T> _commands;


        public CommandInvoker(ILogger logger, ISHProject ishProject, bool enableBackup, string activityDescription)
        {
            Logger = logger;
            IsBackupEnabled = enableBackup;
            ActivityDescription = activityDescription;
            ISHProject = ishProject;
            _commands = new List<T>();
        }

        public void AddCommand(T command)
        {
            _commands.Add(command);
        }

        public virtual void Invoke()
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                var command = _commands[i];
                if (IsBackupEnabled && command is IRestorable)
                {
                    ((IRestorable)command).Backup();
                }
                command.Execute();
                Logger.WriteProgress(ActivityDescription, $"Executed {i}/{_commands.Count}");
            }
        }

        public virtual void Rollback()
        {
            for (int i = _commands.Count - 1; i < _commands.Count; i--)
            {
                var command = _commands[i];
                if (IsBackupEnabled && command is IRestorable)
                {
                    ((IRestorable)command).Rollback();
                }
                Logger.WriteProgress(ActivityDescription, $"Restored {i}/{_commands.Count}");
            }
        }

        public void Clear()
        {
            _commands.Clear();
        }
    }
}
