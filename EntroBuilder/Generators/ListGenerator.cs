using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EntroBuilder.Generators
{
    public class ListGenerator 
    {
        public struct Configuration
        {
            /// <summary>
            /// The probability from 0.0 to 1.0 of producing an empty collection
            /// By default the probability of producing an empty collection is 0.1 (10%).
            /// </summary>
            public double? EmptyProbability { get; set; }

            /// <summary>
            /// If non-empty, howmany items will the collection contain at a minimum
            /// By default the minimum is 1 items.
            /// </summary>
            public int? MinItems { get; set; }

            /// <summary>
            /// If non-empty, howmany items will the collection maximally contain.
            /// By default the maximum is 32 items.
            /// </summary>
            public int? MaxItems { get; set; }
        }

        private readonly Configuration _configuration;
        private readonly Type _elementType;
        private readonly Func<int, IEnumerable> _collectionFactory;
        private readonly Action<IEnumerable, object, Random> _addToCollection;
        private readonly Func<Type, Random, object> _elementFactory;

        public ListGenerator(Configuration configuration, Type listType, Func<Type, Random, object> elementFactory)
        {
            _configuration = configuration;
            _elementFactory = elementFactory;
            _elementType = typeof(object);

            if (listType.IsGenericType())
            {
                if (listType.GetGenericArguments().Length == 1)
                {
                    _elementType = listType.GetGenericArguments().Single();
                }
            }

            _collectionFactory = size =>
            {
                if (listType.IsInterface())
                {
                    var concreteType = typeof(List<>).MakeGenericType(_elementType);
                    return (IList)Activator.CreateInstance(concreteType);
                }
                if (typeof(IList).IsAssignableFrom(listType))
                {
                    return (IList)Activator.CreateInstance(listType);
                }
                throw NotSupported(listType);
            };
            _addToCollection = (list, item, random) =>
            {
                ((IList)list).Add(_elementFactory(_elementType, random));
            };

            if (listType.IsArray)
            {
                _elementType = listType.GetElementType();
                _collectionFactory = size =>
                {
                    var instance = Array.CreateInstance(_elementType, size);
                    return instance;
                };
                int i = 0;
                _addToCollection = (list, item, random) =>
                {
                    ((Array) list).SetValue(item, i);
                    i++;
                };
            }
            else if (listType.IsGenericType())
            {
                var genericTypeDefinition = listType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof (Stack<>))
                {
                    var concreteType = typeof(Stack<>).MakeGenericType(_elementType);
                    _collectionFactory = size => (IEnumerable) Activator.CreateInstance(concreteType);
                    var pushMethod = concreteType.GetMethod("Push");
                    _addToCollection = (list, item, random) =>
                    {
                        pushMethod.Invoke(list, new[] { item });
                    };
                }
                else if (genericTypeDefinition == typeof (ConcurrentBag<>))
                {
                    var concreteType = typeof(ConcurrentBag<>).MakeGenericType(_elementType);
                    _collectionFactory = size => (IEnumerable)Activator.CreateInstance(concreteType);
                    var pushMethod = concreteType.GetMethod("Add");
                    _addToCollection = (list, item, random) =>
                    {
                        pushMethod.Invoke(list, new[] { item });
                    };
                }
                else if ((!listType.IsAbstract() && listType.IsClass()) && genericTypeDefinition.ImplementsGenericInterface(typeof(ICollection<>)))
                {
                    _collectionFactory = size => (IEnumerable)Activator.CreateInstance(listType);
                    var pushMethod = listType.GetMethod("Add");
                    _addToCollection = (list, item, random) =>
                    {
                        pushMethod.Invoke(list, new[] { item });
                    };
                }
            }
        }

        private static Exception NotSupported(Type listType)
        {
            return new NotSupportedException(
                $"{listType} is not supported. Only collections that implement IList, Stack<T> and ConcurrentBag<T> are supported.");
        }

        public IEnumerable Next(Random random)
        {
            var nonEmpty = random.NextDouble() > (_configuration.EmptyProbability ?? 0.1);
            if (nonEmpty)
            {
                var size = random.Next(_configuration.MinItems ?? 1, _configuration.MaxItems ?? 32);
                var list = _collectionFactory(size);
                for (int i = 0; i < size; i++)
                {
                    _addToCollection(list, _elementFactory(_elementType, random), random);
                }
                return list;
            }

            return _collectionFactory(0);
        }
    }
}
