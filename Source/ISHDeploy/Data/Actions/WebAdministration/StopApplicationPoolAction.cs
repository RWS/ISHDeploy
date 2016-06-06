using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.WebAdministration
{
    /// <summary>
    /// Saves files to package.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class StopApplicationPoolAction : BaseAction
    {
        /// <summary>
        /// The Application Pool name.
        /// </summary>
        private readonly string _appPoolName;
        
        /// <summary>
        /// The web Administration manager
        /// </summary>
        private readonly IWebAdministrationManager _webAdminManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecycleApplicationPoolAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appPoolName">Name of the application pool.</param>
        public StopApplicationPoolAction(ILogger logger, string appPoolName)
            : base(logger)
        {
            _appPoolName = appPoolName;

            _webAdminManager = ObjectFactory.GetInstance<IWebAdministrationManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _webAdminManager.StopApplicationPool(_appPoolName);
        }
    }
}
