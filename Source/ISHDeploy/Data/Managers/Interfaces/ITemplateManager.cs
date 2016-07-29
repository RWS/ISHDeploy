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
﻿using System.Collections.Generic;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Generates documents from resource templates.
    /// </summary>
    public interface ITemplateManager
    {
        /// <summary>
        /// Generate output document from the template.
        /// </summary>
        /// <param name="templateFileName">Name of the template file.</param>
        /// <param name="parameters">All parameters that need to be filled out in the template.</param>
        /// <returns></returns>
        string GenerateDocument(string templateFileName, IDictionary<string, string> parameters);
    }
}
