using System;
using System.Collections.Generic;
using System.Diagnostics;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Invokers
{
    public class ActionInvoker : IActionInvoker
    {
        private readonly ILogger _logger;
        private readonly string _activityDescription;
        private readonly List<IAction> _actions;
        
        public ActionInvoker(ILogger logger, string activityDescription)
        {
            _logger = logger;
            _activityDescription = activityDescription;
            _actions = new List<IAction>();
        }

        public void AddAction(IAction action)
        {
            Debug.Assert(action != null, "Action cannot be null");
            _actions.Add(action);
        }

        public void Invoke()
        {
            IAction action = null;

            try
            {
                for (var i = 0; i < _actions.Count; i++)
                {
                    action = _actions[i];

                    action.Execute();

                    _logger.WriteProgress(_activityDescription, $"Executed {i} of {_actions.Count} actions");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, action);
                
                throw;
            }
        }
    }
}
