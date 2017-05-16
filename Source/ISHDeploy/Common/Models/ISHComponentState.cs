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

using System;
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// <para type="description">Represents the state of deployment's component.</para>
    /// </summary>
    public class ISHComponent
    {
        /// <summary>
        /// The name of InfoShare component.
        /// </summary>
        public ISHComponentName Name { get; }

        /// <summary>
        /// The state of InfoShare component.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponent"/> class in vanilla state.
        /// </summary>
        /// <param name="name">The name of InfoShare component</param>
        public ISHComponent(ISHComponentName name)
        {
            Name = name;
            IsEnabled = (
                name == ISHComponentName.CM ||
                name == ISHComponentName.WS ||
                name == ISHComponentName.STS ||
                name == ISHComponentName.COMPlus);
        }

        /// <summary>
        /// Change state of IsEnabled property
        /// </summary>
        /// <param name="enabled"></param>
        public void SetIsEnabled(bool enabled)
        {
            IsEnabled = enabled;
        }
    }

    /// <summary>
    /// <para type="description">Represents collection of Components with their states.</para>
    /// </summary>
    public class ISHComponentsCollection : IEnumerable<ISHComponent>
    {
        /// <summary>
        /// List of components
        /// </summary>
        private readonly List<ISHComponent> _components;

        /// <summary>
        /// Get component by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ISHComponent by ISHComponentName</returns>
        public ISHComponent this[ISHComponentName name]
        {
            get { return _components.SingleOrDefault(x => x.Name == name); }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponentsCollection"/> class.
        /// </summary>
        public ISHComponentsCollection()
        {
            _components = (from ISHComponentName name in Enum.GetValues(typeof (ISHComponentName))
                select new ISHComponent(name)).ToList();
        }

        /// <summary>
        /// Get enumerator for collection
        /// </summary>
        /// <returns>Enumerator for collection</returns>
        public IEnumerator<ISHComponent> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator for collection
        /// </summary>
        /// <returns>Enumerator for collection</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}