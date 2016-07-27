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
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// The path to ~\Web\Author\ASP\Tree.htm
        /// </summary>
        protected ISHFilePath AuthorASPTreeHtmPath
        {
            get
            {
                return new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\Tree.htm");
            }
        } 

        /// <summary>
        /// The path to ~\Web\Author\ASP\Tree.htm
        /// </summary>
        protected class AuthorASPTreeHtm
        {
            /// <summary>
            /// The translation job hack comment placeholder
            /// </summary>
            public const string TranslationJobHack = "//Translation Jobs hack";
        }
    }
}
