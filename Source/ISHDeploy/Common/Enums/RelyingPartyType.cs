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
    /// Operation type enum
    ///	<para type="description">Enumeration of Relying parties Types.</para>
    /// </summary>
    public enum RelyingPartyType
    {
        /// <summary>
        /// Flag to identify that none prefixes should be set
        /// </summary>
        None,

        /// <summary>
        /// Flag to identify Info Share
        /// Used in only seeded configurations
        /// </summary>
        ISH,

        /// <summary>
        /// Flag to identify Live Content
        /// </summary>
        LC,

        /// <summary>
        /// Flag to identify Blue Lion
        /// </summary>
        BL
    }
}