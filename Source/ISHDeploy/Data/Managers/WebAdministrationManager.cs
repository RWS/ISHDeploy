using System;
using System.Linq;
using Microsoft.Web.Administration;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    /// <seealso cref="IWebAdministrationManager" />
    public class WebAdministrationManager : IWebAdministrationManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAdministrationManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WebAdministrationManager(ILogger logger)
        {
            _logger = logger;

        }

        /// <summary>
        /// Recycles specific application pool
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <param name="startIfNotRunning">if set to <c>true</c> then starts application pool if not running.</param>
        public void RecycleApplicationPool(string applicationPoolName, bool startIfNotRunning = false)
        {
            _logger.WriteDebug($"Recycling application pool: `{applicationPoolName}`");

            using (ServerManager manager = new ServerManager())
            {
                ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == applicationPoolName);

                if (appPool != null)
                {
                    // Wait while application pool operation is completed
                    while (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    //The app pool is running, so stop it first.
                    if (appPool.State == ObjectState.Started)
                    {
                        _logger.WriteDebug($"Application pool `{applicationPoolName}` is recycling.");

                        appPool.Recycle();
                    }
                    else if (appPool.State == ObjectState.Stopped && startIfNotRunning)
                    {
                        _logger.WriteDebug($"Application pool `{applicationPoolName}` is starting.");

                        appPool.Start();
                    }
                }
                else
                {
                    _logger.WriteWarning($"Application pool `{applicationPoolName}` does not exists.");
                }
            }
        }
    }
}
