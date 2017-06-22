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

using System;
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHComponent;

namespace ISHDeploy.Cmdlets.ISHServiceFullTextIndex
{
    /// <summary>
    /// <para type="synopsis">Sets target lucene Uri.</para>
    /// <para type="description">The Set-ISHServiceFullTextIndex cmdlet sets target lucene Uri.</para>
    /// <para type="link">Enable-ISHServiceFullTextIndex</para>
    /// <para type="link">Disable-ISHServiceFullTextIndex</para>
    /// <para type="link">Get-ISHServiceFullTextIndex</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceFullTextIndex -ISHDeployment $deployment -Uri "http://127.0.0.1:8080/solr/"</code>
    /// <para>This command changes the target Uri of instances of SolrLucene services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHServiceFullTextIndex")]
    public sealed class SetISHServiceFullTextIndexCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The target lucene Uri.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The target lucene Uri")]
        [ValidateNotNullOrEmpty]
        public Uri Uri { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SetISHServiceFullTextIndexOperation(Logger, ISHDeployment, Uri);

            operation.Run();
        }
    }
}
