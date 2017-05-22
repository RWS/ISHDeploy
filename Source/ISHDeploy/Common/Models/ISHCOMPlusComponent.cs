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

using ISHDeploy.Common.Enums;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// <para type="description">Represents the installed COM+ components that deployment is used.</para>
    /// </summary>
    public class ISHCOMPlusComponent
    {
        /// <summary>
        /// The name of COM+ component.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The status of COM+ component.
        /// </summary>
        public ISHCOMPlusComponentStatus Status { get; set; }
    }
}
