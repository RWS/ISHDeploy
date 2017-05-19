using System;
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// <para type="description">Represents collection of components.</para>
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
        /// <param name="name">The name of component</param>
        /// <returns>ISHComponent by ISHComponentName</returns>
        public ISHComponent this[ISHComponentName name]
        {
            get
            {
                return _components.SingleOrDefault(x => x.Name == name);
            }
        }

        /// <summary>
        /// Get component by name
        /// </summary>
        /// <param name="name">The name of component</param>
        /// <param name="role">The component's role</param>
        /// <returns>ISHComponent by ISHComponentName</returns>
        public ISHComponent this[ISHComponentName name, ISHBackgroundTaskRole role]
        {
            get
            {
                return _components.SingleOrDefault(x => x.Name == name && x.Role != null && x.Role == role.ToString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponentsCollection"/> class in vanilla state.
        /// </summary>
        public ISHComponentsCollection()
        {
            _components = new List<ISHComponent>();
            foreach (ISHComponentName name in Enum.GetValues(typeof(ISHComponentName)))
            {
                _components.Add(new ISHComponent
                {
                    Name = name,
                    IsEnabled = (
                        name == ISHComponentName.CM ||
                        name == ISHComponentName.WS ||
                        name == ISHComponentName.STS ||
                        name == ISHComponentName.COMPlus),
                    Role = name == ISHComponentName.BackgroundTask ? "Default" : null 
                });
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponentsCollection"/> class.
        /// </summary>
        public ISHComponentsCollection(IEnumerable<ISHComponent> components)
        {
            _components = new List<ISHComponent>(components);
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