using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Providers
{
    public class ISHProjectProvider
    {
        private ISHProject _ishProject;

        static ISHProjectProvider()
        {
            Instance = new ISHProjectProvider();
        }

        public static ISHProjectProvider Instance { get; }

        public void InitializeIshProject(ISHProject ishProject)
        {
            _ishProject = ishProject;
        }

        public ISHProject IshProject => _ishProject;
    }
}
