using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntroBuilder.FallbackGenerators
{
    public class DefaultInterfaceImplementationFallbackGenerator : IFallbackGenerator
    {
        public DefaultInterfaceImplementationFallbackGenerator() { }

        readonly IEnumerable<Assembly> _assemblies;
        public DefaultInterfaceImplementationFallbackGenerator(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public bool TryNext(Type type, Random random, out object instance)
        {
            if (type.IsInterface())
            {
                var assemblies = _assemblies ?? new[] { type.GetTypeInfo().Assembly };

                var firstConcreteType = assemblies
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(x => x.IsClass() && !x.IsAbstract() && type.IsAssignableFrom(x));

                if (firstConcreteType == null)
                {
                    instance = null;
                    return false;
                }

                instance = Activator.CreateInstance(firstConcreteType, true);
                return true;
            }

            instance = null;
            return false;
        }
    }
}
