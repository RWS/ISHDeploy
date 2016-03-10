using System;
using System.Collections.Generic;

namespace InfoShare.Deployment
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

            throw new ArgumentException("Such type is not registered.");
        }
    }
}
