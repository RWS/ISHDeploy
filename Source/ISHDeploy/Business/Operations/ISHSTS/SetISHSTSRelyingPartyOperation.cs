/**
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

using System.Collections.Generic;
using ISHDeploy.Business.Invokers;
﻿using ISHDeploy.Data.Actions.DataBase;
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Models.Structs.SQL;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets the Relying Party
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class SetISHSTSRelyingPartyOperation : BasePathsOperation
    {
        /*
        <# 
         .Synopsis
          Adds a Relying Party to InfoShareSTS configuration.

         .Description
          Adds a Relying Party to InfoShareSTS configuration..

         .Parameter Name
          The name of the Relying Party

         .Parameter Realm
          The realm of the Relying Party.

         .Parameter EncryptingCertificate
          The encrypting certificate if the relying party is for a WCF service.

         .Example
           AddRelyingParty InfoShareAuthor https://url/InfoShareAuthor/

        #>
        function Add-RelyingParty($name,$realm,$encryptingCertificate)
        {
            #Trace
            $parametersMessage=RenderKeyValue "name" $name
            $parametersMessage+=RenderKeyValue "realm" $realm
            $parametersMessage+=RenderKeyValue "encryptingCertificate" $encryptingCertificate
            WriteVerboseWithHeaderFooter "Add-RelyingParty Parameters" $parametersMessage

            #Prepare Database Connection and Command
            $connection = New-Object "System.Data.SqlServerCe.SqlCeConnection" $connectionString
            $addCommand = New-Object "System.Data.SqlServerCe.SqlCeCommand"
            $addCommand.CommandType = [System.Data.CommandType]::Text
            $addCommand.Connection = $connection
            $parameter=$addCommand.Parameters.Add("@name",$name)
            $parameter=$addCommand.Parameters.Add("@realm",$realm)
            if($encryptingCertificate -eq $null)
            {
                $parameter=$addCommand.Parameters.Add("encryptingCertificate",[System.DBNull]::Value)
            }
            else
            {
                $parameter=$addCommand.Parameters.Add("encryptingCertificate",$encryptingCertificate)	
            }
            $addCommand.CommandText = "INSERT RelyingParties (Name,Enabled,Realm,TokenLifeTime,EncryptingCertificate) VALUES (@name,1,@realm,0,@encryptingCertificate)"

            $existCommand = New-Object "System.Data.SqlServerCe.SqlCeCommand"
            $existCommand.CommandType = [System.Data.CommandType]::Text
            $existCommand.Connection = $connection
            $parameter=$existCommand.Parameters.Add("@realm",$realm)
            $existCommand.CommandText = "SELECT COUNT(Name) FROM RelyingParties WHERE Realm=@realm"

            #Execute Command
            try
            {
                $connection.Open()
                $existingRelyingPartiesCount=$existCommand.ExecuteScalar()

                if($existingRelyingPartiesCount -eq 0)
                {
                    $addedRelyingPartiesCount=$addCommand.ExecuteNonQuery()
                }
                else
                {
                    Write-Warning "Relying Party $realm already exists"
                }
            }
            finally
            {
                $connection.Close()
                $connection.Dispose()
            }
            if($addedRelyingPartiesCount -ge 1)
            {
                Write-Host "Relying Party $realm added"
            } 
        }
        */

        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHSTSRelyingPartyOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Realm">The realm.</param>
        /// <param name="RPtype">The relying party type.</param>
        /// <param name="EncryptionCertificate">The encryption certificate.</param>
        public SetISHSTSRelyingPartyOperation(ILogger logger, Models.ISHDeployment ishDeployment, string Name, string Realm, string RPtype, string EncryptionCertificate):
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Getting the path to the packages folder");

            string sqlQuery = InfoShareSTSDataBase.InsertUpdateRelyingPartySQLCommandFormat;

            _invoker.AddAction(new SqlCompactUpdateAction(logger,
                    InfoShareSTSDataBase.ConnectionString,
                    sqlQuery,
                    new List<object>()));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
