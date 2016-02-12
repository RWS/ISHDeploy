using System;
using System.Collections.Generic;
using System.Diagnostics;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Business.Invokers
{
    public class CommandInvoker : ICommandInvoker
    {
        private readonly ILogger _logger;
        private readonly string _activityDescription;
        private readonly List<ICommand> _commands;
        
        public CommandInvoker(ILogger logger, string activityDescription)
        {
            _logger = logger;
            _activityDescription = activityDescription;
            _commands = new List<ICommand>();
        }

        public void AddCommand(ICommand command)
        {
            Debug.Assert(command != null, "Command cannot be null");
            _commands.Add(command);
        }

        public void Invoke()
        {
            ICommand command = null;

            try
            {
                for (var i = 0; i < _commands.Count; i++)
                {
                    command = _commands[i];
                    
                    command.Execute();

                    _logger.WriteProgress(_activityDescription, $"Executed {i} of {_commands.Count} commands");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, command);
                
                throw;
            }
        }
    }
}
