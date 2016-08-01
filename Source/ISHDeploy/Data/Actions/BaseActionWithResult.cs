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
﻿using System;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions
{
    /// <summary>
    /// Base class for action that can be executed and return execution result.
    /// </summary>
    /// <typeparam name="TResult">The type of the execution result.</typeparam>
    /// <seealso cref="BaseAction" />
    public abstract class BaseActionWithResult<TResult> : BaseAction
    {
        /// <summary>
        /// The execution result.
        /// </summary>
        protected readonly Action<TResult> ReturnResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseActionWithResult{TResult}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="returnResult">The delegate that returns execution result.</param>
        protected BaseActionWithResult(ILogger logger, Action<TResult> returnResult) : base(logger)
        {
            ReturnResult = returnResult;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns></returns>
        protected abstract TResult ExecuteWithResult();

        /// <summary>
        /// Executes current action.
        /// </summary>
        public sealed override void Execute()
        {
            var result = ExecuteWithResult();

            ReturnResult(result);
        }
    }
}
