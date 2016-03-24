namespace ISHDeploy.Interfaces.Actions
{
    /// <summary>
    /// Provides ability to backup and restore actions.
    /// </summary>
    public interface IRestorableAction
    {
		/// <summary>
		///	Creates backup of the asset.
		/// </summary>
        void Backup();
		
		/// <summary>
		///	Reverts an asset to initial state.
		/// </summary>
		void Rollback();
    }
}
