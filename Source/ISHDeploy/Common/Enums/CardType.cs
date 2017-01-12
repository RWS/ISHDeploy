/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace ISHDeploy.Common.Enums
{
    /// <summary>
    /// Enumeration of Card Types
    /// <para type="description">Enumeration of Card Type</para>
    /// </summary>
    [Flags]
    public enum CardType
    {
        /// <summary>
        /// ISHIllustration card type.
        /// </summary>
        ISHIllustration,
        /// <summary>
        /// ISHModule card type.
        /// </summary>
        ISHModule,
        /// <summary>
        /// ISHMasterDoc card type.
        /// </summary>
        ISHMasterDoc,
        /// <summary>
        /// ISHTemplate card type.
        /// </summary>
        ISHTemplate,
        /// <summary>
        /// ISHLibrary card type.
        /// </summary>
        ISHLibrary,
        /// <summary>
        /// ISHReference card type.
        /// </summary>
        ISHReference,
        /// <summary>
        /// ISHQuery card type.
        /// </summary>
        ISHQuery,
        /// <summary>
        /// ISHPublication card type.
        /// </summary>
        ISHPublication
    }
}
