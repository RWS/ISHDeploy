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
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions
{
    /// <summary>
    /// Base class for action that can be executed.
    /// </summary>
    /// <seealso cref="IAction" />
    public abstract class BaseAction : IAction
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BaseAction(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public abstract void Execute();
    }
}
