using System;
using System.Collections.Generic;
using System.Reflection;

namespace InfoShare.Deployment
{
    public class ObjectFactory
    {
        private static readonly IDictionary<Type, Type> _types = new Dictionary<Type, Type>();
        private static readonly IDictionary<Type, object> _typeInstances = new Dictionary<Type, object>();

        public static void SetInstance<TContract, TImplementation>(TImplementation instance)
        {
            _typeInstances[typeof(TContract)] = instance;
        }

        public static T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        private static object GetInstance(Type contract)
        {
            if (_typeInstances.ContainsKey(contract))
            {
                return _typeInstances[contract];
            }
            else
            {
                Type implementation = _types[contract];
                ConstructorInfo constructor = implementation.GetConstructors()[0];
                ParameterInfo[] constructorParameters = constructor.GetParameters();
                if (constructorParameters.Length == 0)
                {
                    return Activator.CreateInstance(implementation);
                }
                List<object> parameters = new List<object>(constructorParameters.Length);
                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    parameters.Add(GetInstance(parameterInfo.ParameterType));
                }
                return constructor.Invoke(parameters.ToArray());
            }
        }
    }
}
