using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.WebAdministration
{
    /// <summary>
    /// Saves files to package.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class RecycleApplicationPoolAction : BaseAction
    {
        /// <summary>
        /// The Application Pool name.
        /// </summary>
        private readonly string _appPoolName;

        /// <summary>
        /// The parameter determines whether application pool should be started if it was not running before
        /// </summary>
        private readonly bool _startIfNotRunning;

        /// <summary>
        /// The web Administration manager
        /// </summary>
        private readonly IWebAdministrationManager _webAdminManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecycleApplicationPoolAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appPoolName">Name of the application pool.</param>
        /// <param name="startIfNotRunning">if set to <c>true</c> then starts application pool if not running.</param>
        public RecycleApplicationPoolAction(ILogger logger, string appPoolName, bool startIfNotRunning = false)
            : base(logger)
        {
            _appPoolName = appPoolName;
            _startIfNotRunning = startIfNotRunning;

            _webAdminManager = ObjectFactory.GetInstance<IWebAdministrationManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _webAdminManager.RecycleApplicationPool(_appPoolName, _startIfNotRunning);
        }
    }
}
