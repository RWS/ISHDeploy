using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHDeploy.Models
{
    class DeploymentPartial
    {

        public DeploymentPartial(ISHDeployment iSHDeployment)
        {
            SoftwareVersion = iSHDeployment.SoftwareVersion.ToString();
            Name = iSHDeployment.Name;
            AppPath = iSHDeployment.AppPath;
            WebPath = iSHDeployment.WebPath;
            DataPath = iSHDeployment.DataPath;
            ConnectString = iSHDeployment.ConnectString;
            DatabaseType = iSHDeployment.DatabaseType;
            WebAppNameCM = iSHDeployment.WebNameCM;
            WebAppNameWS = iSHDeployment.WebNameWS;
            WebAppNameSTS = iSHDeployment.WebNameSTS;
            AccessHostName = iSHDeployment.AccessHostName;
        }
        public String SoftwareVersion { get; set; }
        public String Name { get; set; }
        public String AppPath { get; set; }
        public String WebPath { get; set; }
        public String DataPath { get; set; }
        public String ConnectString { get; set; }
        public String DatabaseType { get; set; }
        public String WebAppNameCM { get; set; }
        public String WebAppNameWS { get; set; }
        public String WebAppNameSTS { get; set; }
        public String AccessHostName { get; set; }
    }
}
