using System;
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHIntegrationDB;

namespace ISHDeploy.Cmdlets.ISHIntegrationDB
{
    /// <summary>
    /// <para type="synopsis">Generates SQL Server script that grants necessary permissions.</para>
    /// <para type="description">The Save-ISHIntegrationDBSTSSQLServerConfigurationCmdlet generates SQL Server script that grants necessary permissions into package folder.</para>
    /// <para type="description">Cmdlet will overwrite the file if there is already one with same name.</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.BaseHistoryEntryCmdlet" />
    /// <example>
    ///   <code>PS C:\&gt;Save-ISHIntegrationDBSTSSQLServerConfigurationCmdlet -ISHDeployment $deployment -FileName GrantPermissions.sql</code>
    ///   <para>Generates SQL script that grants necessary permissions.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// </para>
    /// </example>
    /// <example>
    ///   <code>PS C:\&gt;Save-ISHIntegrationDBSTSSQLServerConfigurationCmdlet -ISHDeployment $deployment -FileName GrantPermissions.ps1 -Type PS1</code>
    ///   <para>Generates PS1 script that runs sql command on server and grants necessary permissions.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// </para>
    /// </example>
    [Cmdlet(VerbsData.Save, "ISHIntegrationDBSTSSQLServerConfiguration")]
    public class SaveISHIntegrationDBSTSSQLServerConfigurationCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Name of the output file.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Name of the output file.")]
        public string FileName { get; set; }


        /// <summary>
        /// <para type="description">Specifies the output file type. Default value is 'SQL'.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Select output file type, either `PS1` or `SQL`.")]
        public OutputType Type { get; set; } = OutputType.SQL;

        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (ISHDeployment.DatabaseType == "oracle")
            {
                throw new ArgumentException($"Save-ISHIntegrationDBSTSSQLServerConfiguration Commandlet is not supported for `Oracle` Database type.");
            }

            var operation = new SaveISHIntegrationDBSTSSQLServerConfigurationOperation(Logger, ISHDeployment, FileName, Type);

            operation.Run();
        }
    }
}
