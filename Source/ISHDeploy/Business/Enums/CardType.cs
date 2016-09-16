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

namespace ISHDeploy.Business.Enums
{
    public enum CardType
    {
        /// <summary>
        /// No objects allowed except subfolders (VDOCTYPENONE)
        /// </summary>
        VDOCTYPENONE,
        /// <summary>
        /// Modules/Maps/Topics (VDOCTYPEMAP)
        /// </summary>
        VDOCTYPEMAP,
        /// <summary>
        /// MasterDocuments/Masters/BookMaps/DocumentOutlines (VDOCTYPEMASTER)
        /// </summary>
        VDOCTYPEMASTER,
        /// <summary>
        /// Libraries (VDOCTYPELIB)
        /// </summary>
        VDOCTYPELIB,
        /// <summary>
        /// Templates (VDOCTYPETEMPLATE)
        /// </summary>
        VDOCTYPETEMPLATE,
        /// <summary>
        /// Illustrations/Images/Graphics (VDOCTYPEILLUSTRATION)
        /// </summary>
        VDOCTYPEILLUSTRATION,
        /// <summary>
        /// Publication objects (VDOCTYPEPUBLICATION)
        /// </summary>
        VDOCTYPEPUBLICATION,
        /// <summary>
        /// Document references (VDOCTYPEREFERENCE)
        /// </summary>
        VDOCTYPEREFERENCE,
        /// <summary>
        /// Query Folders (VDOCTYPEQUERY)
        /// </summary>
        VDOCTYPEQUERY,
    }
}
