using ISHDeploy.Common;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Remove (all) dependencies for the windows service
    /// </summary>
    public class RemoveWindowsServiceDependencyAction : BaseAction
    {
        /// <summary>
        /// The name of deployment service.
        /// </summary>
        private readonly string _serviceName;

        /// <summary>
        /// The windows service manager
        /// </summary>
        private readonly IWindowsServiceManager _serviceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveWindowsServiceDependencyAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">The deployment service</param>
        public RemoveWindowsServiceDependencyAction(ILogger logger, ISHWindowsService service)
            : base(logger)
        {
            _serviceName = service.Name;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _serviceManager.RemoveWindowsServiceDependency(_serviceName);

        }
    }
}
