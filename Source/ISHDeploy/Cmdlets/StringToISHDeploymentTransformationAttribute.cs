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
using System.Management.Automation;
using System;
using System.Linq;
using ISHDeploy.Business.Operations.ISHDeployment;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Trasform input parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringToISHDeploymentTransformationAttribute : ArgumentTransformationAttribute
    {
        /// <summary>
        /// Trasform ISHDeployment input parameter from string 
        /// </summary>
        /// <param name="engineIntrinsics"></param>
        /// <param name="inputData"> ISHDeployment object</param>
        /// <returns></returns>
        public override Object Transform(EngineIntrinsics engineIntrinsics, Object inputData)
        {
            if (inputData is string)
            {
                var operation = new GetISHDeploymentsOperation(CmdletsLogger.Instance(), inputData.ToString());
                return operation.Run().FirstOrDefault();
            }

            if ((((PSObject)inputData).BaseObject).GetType() == typeof(Models.ISHDeployment))
            {
                return inputData;
            }

            throw new ArgumentTransformationMetadataException("Type of the object is not ISHDeployment or a name of it.");
        }
    }
}