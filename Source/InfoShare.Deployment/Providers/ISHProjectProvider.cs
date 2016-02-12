using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Providers
{
    public class ISHProjectProvider
    {
        private ISHDeployment _ishDeployment;

        static ISHProjectProvider()
        {
            Instance = new ISHProjectProvider();
        }

        public static ISHProjectProvider Instance { get; }

        public void InitializeIshProject(ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

        public ISHDeployment ISHDeployment => _ishDeployment;
    }
}
