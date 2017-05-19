using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// <para type="description">Represents collection of components.</para>
    /// </summary>
    [XmlRoot("ISHComponents", Namespace = "")]
    public class ISHComponentsCollection
    {
        /// <summary>
        /// List of components
        /// </summary>
        [XmlElement("ISHComponent", Namespace = "")]
        public List<ISHComponent> Components { get; set; }

        /// <summary>
        /// Get component by name
        /// </summary>
        /// <param name="name">The name of component</param>
        /// <returns>ISHComponent by ISHComponentName</returns>
        public ISHComponent this[ISHComponentName name]
        {
            get
            {
                return Components.SingleOrDefault(x => x.Name == name);
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
                return Components.SingleOrDefault(x => x.Name == name && x.Role != null && x.Role == role.ToString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponentsCollection"/> class.
        /// </summary>
        public ISHComponentsCollection()
        {
            Components = new List<ISHComponent>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponentsCollection"/> class. If isVanilla = True init vanilla state of components.
        /// </summary>
        /// <param name="isVanilla">Init vanilla state</param>
        public ISHComponentsCollection(bool isVanilla)
        {
            Components = new List<ISHComponent>();
            if (isVanilla)
            {
                Components = new List<ISHComponent>();
                foreach (ISHComponentName name in Enum.GetValues(typeof(ISHComponentName)))
                {
                    Components.Add(new ISHComponent
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHComponentsCollection"/> class.
        /// </summary>
        /// <param name="components">The initial capacity</param>
        public ISHComponentsCollection(IEnumerable<ISHComponent> components)
        {
            Components = new List<ISHComponent>(components);
        }

        /// <summary>
        /// Get enumerator for collection
        /// </summary>
        /// <returns>Enumerator for collection</returns>
        public IEnumerator<ISHComponent> GetEnumerator()
        {
            return Components.GetEnumerator();
        }
    }
}