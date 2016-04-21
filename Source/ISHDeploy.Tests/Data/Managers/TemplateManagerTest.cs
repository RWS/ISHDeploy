using System;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISHDeploy.Tests.Data.Managers
{
    [TestClass]
    public class TemplateManagerTest : BaseUnitTest
    {
        [TestMethod]
        public void SaveCMSecurityTokenService_Save_works()
        {
            ObjectFactory.SetInstance<IFileManager>(new FileManager(Logger));
            var templateManager = new TemplateManager(Logger);

            var wsIds = new[]
            {
                "https://win-bcmco6u3oi4.global.sdl.corp/ISHWSSQL2014",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/Application.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/Baseline.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/DocumentObj.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/EDT.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/EventMonitor.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/Folder.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/ListOfValues.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/MetadataBinding.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/OutputFormat.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/PublicationOutput.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/Search.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/Settings.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/TranslationJob.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/TranslationTemplate.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/User.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/UserGroup.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API25/UserRole.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Application.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/DocumentObj.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/EDT.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Folder.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/MetaDataAssist.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/OutputFormat.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Publication.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/PublicationOutput.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Reports.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Search.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Settings.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API20/Workflow.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API/Application.svc",
                "https://WIN-BCMCO6U3OI4.global.sdl.corp/ISHWSSQL2014/Wcf/API/ConditionManagement.svc",
            };

            templateManager.SaveCMSecurityTokenService("GeneratedFile.md", "https://win-bcmco6u3oi4.global.sdl.corp/ISHCMSQL2014", wsIds, "ce6d03f0f1eb8d581078305c749f8bada486e8fe");
        }
    }
}
