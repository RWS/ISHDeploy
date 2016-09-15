/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using ISHDeploy.Business.Operations.ISHUIOperation;
using ISHDeploy.Models.UI;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIComponents
{
    /// <summary>
    /// <para type="synopsis">Remove main menu item.</para>
    /// <para type="description">The Remove-ISHUIMainMenuButton cmdlet remove exist menu item.</para>
    /// <para type="link">Move-ISHUIMainMenuButton</para>    
    /// <para type="link">Set-ISHUIMainMenuButton</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Remove-ISHUIMainMenuButton -ISHDeployment $deployment -Label "Inbox2"</code>
    /// <para>This command remove main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "ISHUIMainMenuButton")]
    public sealed class RemoveISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var model = new MainMenuModel(Label);
            var operation = new RemoveUIOperation(Logger, ISHDeployment, model);
            operation.Run();
        }
    }

}
