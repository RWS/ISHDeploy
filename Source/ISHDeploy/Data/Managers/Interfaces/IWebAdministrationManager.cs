namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    public interface IWebAdministrationManager
    {
        /// <summary>
        /// Recycles specific application pool
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <param name="startIfNotRunning">if set to <c>true</c> then starts application pool if not running.</param>
        void RecycleApplicationPool(string applicationPoolName, bool startIfNotRunning = false);
    }
}
