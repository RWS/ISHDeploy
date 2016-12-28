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
using System.Collections.Generic;

namespace ISHDeploy.Common
{
    /// <summary>
    /// Represents the simplest implementation of the IoC container. It can register and return instances of classes.
    /// </summary>
    public class ObjectFactory
    {
        /// <summary>
        /// The registered type instances.
        /// </summary>
        private static readonly IDictionary<Type, object> TypeInstances = new Dictionary<Type, object>();

        /// <summary>
        /// Registers the instance of the type.
        /// </summary>
        /// <typeparam name="TContract">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        public static void SetInstance<TContract>(TContract instance)
        {
            TypeInstances[typeof(TContract)] = instance;
        }

        /// <summary>
        /// Gets the instance by registered type.
        /// </summary>
        /// <typeparam name="T">The registered type.</typeparam>
        /// <returns>Instance of specific type.</returns>
        public static T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        /// <summary>
        /// Gets the instance by registered type.
        /// </summary>
        /// <param name="contract">The registered type.</param>
        /// <returns>Instance of specific type.</returns>
        private static object GetInstance(Type contract)
        {
            if (TypeInstances.ContainsKey(contract))
            {
                return TypeInstances[contract];
            }

            throw new ArgumentException($"Such type `{contract}` is not registered.");
        }
    }
}
