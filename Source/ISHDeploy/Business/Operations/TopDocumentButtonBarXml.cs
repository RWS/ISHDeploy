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

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// Provides constants related to ~\Web\Author\ASP\XSL\TopDocumentButtonbar.xml
        /// </summary>
        protected class TopDocumentButtonBarXml
        {
            /// <summary>
            /// The translation job attribute value
            /// </summary>
            public const string TranslationJobAttribute = "NAME=\"TranslationJob\"";

            /// <summary>
            /// The top document buttonbar xpath
            /// </summary>
            public const string TopDocumentTranslationJobXPath = "BUTTONBAR/BUTTON[INPUT[@NAME = 'TranslationJob']]";
        }
    }
}
