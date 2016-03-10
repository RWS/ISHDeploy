namespace InfoShare.Deployment.Interfaces.Actions
{
    /// <summary>
    /// Provides ability to be executed.
    /// </summary>
    public interface IAction
    {
		/// <summary>
		/// Executes current action.
		/// </summary>
        void Execute();
    }
}
