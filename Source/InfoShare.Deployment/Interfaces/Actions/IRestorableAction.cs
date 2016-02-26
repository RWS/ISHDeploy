namespace InfoShare.Deployment.Interfaces.Actions
{
    public interface IRestorableAction
    {
		/// <summary>
		///	Creates backup of the asset
		/// </summary>
        void Backup();
		
		/// <summary>
		///	Reverts an asset to initial state
		/// </summary>
		void Rollback();
    }
}
