using System;
using System.Collections.Generic;
using System.Diagnostics;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Invokers
{
    /// <summary>
    /// Executes the sequence of actions one by one
    /// </summary>
    public class ActionInvoker : IActionInvoker
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Description for general activity that to be done
        /// </summary>
        private readonly string _activityDescription;

        /// <summary>
        /// Sequence of the actions that <see cref="T:InfoShare.Deployment.Business.Invokers.ActionInvoker"/> going to execute
        /// </summary>
        private readonly List<IAction> _actions;
        
        /// <summary>
        /// Constuctor for <see cref="T:InfoShare.Deployment.Business.Invokers.ActionInvoker"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="T:InfoShare.Deployment.Interfaces.ILogger"/></param>
        /// <param name="activityDescription">Description of the general activity to be done</param>
        public ActionInvoker(ILogger logger, string activityDescription)
        {
            _logger = logger;
            _activityDescription = activityDescription;
            _actions = new List<IAction>();
        }

        /// <summary>
        /// Adds action to the sequence
        /// </summary>
        /// <param name="action">New action in the sequence</param>
        public void AddAction(IAction action)
        {
            Debug.Assert(action != null, "Action cannot be null");
            _actions.Add(action);
        }

		/// <summary>
		/// Executes sequence of actions one by one
		/// </summary>
        public void Invoke()
        {
            _logger.WriteDebug($"Entered Invoke method for {nameof(ActionInvoker)}");
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

                    var actionNumber = i + 1;
                    _logger.WriteProgress(_activityDescription, $"Executed {actionNumber} of {_actions.Count} actions", (int)(actionNumber / (double)_actions.Count * 100));
                }
            }
            catch (Exception ex)
            {
				// To do a rollback we need to do it in a sequance it was executed, thus we should reverse list.
				//if (isRollbackAllowed)
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
