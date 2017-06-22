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
namespace ISHDeploy.Common.Enums
{
    /// <summary>
    /// <para type="description">Specify the name of registry values.</para>
    /// </summary>
    public enum RegistryValueName
    {
        /// <summary>
        /// Db connection string
        /// </summary>
        Connect,

        /// <summary>
        /// Database type
        /// </summary>
        ComponentName,

        /// <summary>
        /// SolrLuceneBaseUrl
        /// </summary>
        SolrLuceneBaseUrl,

        /// <summary>
        /// DependOnService
        /// </summary>
        DependOnService,

        /// <summary>
        /// SolrLuceneServicePort
        /// </summary>
        SolrLuceneServicePort,

        /// <summary>
        /// SolrLuceneStopPort
        /// </summary>
        SolrLuceneStopPort,

        /// <summary>
        /// SolrLuceneStopKey
        /// </summary>
        SolrLuceneStopKey
    }
}
