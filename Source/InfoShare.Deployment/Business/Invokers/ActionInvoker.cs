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

        public void Invoke(bool isRollbackAllowed = true)
        {
            IAction action = null;

			List<IAction> executedActions = new List<IAction>();
	        try
	        {
		        for (var i = 0; i < _actions.Count; i++)
		        {
			        action = _actions[i];

			        action.Execute();

				    // If action was executed, we`re adding it to the list to make a rollback if neccesary;
				    executedActions.Add(action);

			        _logger.WriteProgress(_activityDescription, $"Executed {i} of {_actions.Count} actions");
		        }
	        }
	        catch (Exception ex)
	        {
				// To do a rollback we need to do it in a sequance it was executed, thus we should reverse list.

				if (isRollbackAllowed)
		        {
			        executedActions.Reverse();
					executedActions.ForEach(x =>
					{
						(x as IRestorableAction)?.Rollback();
					});
				}

				_logger.WriteError(ex, action);

		        throw;
	        }
	        finally
	        {
		        executedActions = null;
	        }
        }
    }
}
