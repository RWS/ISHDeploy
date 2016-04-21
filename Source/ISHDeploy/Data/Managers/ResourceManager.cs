using System;
using System.IO;
using System.Reflection;

namespace ISHDeploy.Data.Managers
{
    public class ResourceManager
    {
        public string GetResourceContent(string resPath)
        {
            string resContent;

            using (var resourceReader = Assembly.GetExecutingAssembly().GetManifestResourceStream(resPath))
            {
                if (resourceReader == null)
                {
                    throw new ArgumentException($"Invalid resource path: {resPath}");
                }

                using (var reader = new StreamReader(resourceReader))
                {
                    resContent = reader.ReadToEnd();
                }
            }

            return resContent;
        }
    }
}
