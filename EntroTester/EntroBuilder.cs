using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntroTester
{
    public delegate bool TryGet<in TParam, TOutput>(TParam param, out TOutput output);

    public class EntroBuilder<T>  where T : class, new()
    {
        const int Seed = 0x1;
        Random _random;

        public EntroBuilder(int? seed = null)
        {
            _random = new Random(seed ?? Seed);
        }

        readonly Dictionary<PropertyInfo, IGenerator> _possibleValueSelectors = new Dictionary<PropertyInfo, IGenerator>();
        public EntroBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IGenerator<TProperty> generator)
        {
            var propertyInfo = propertyExpression.GetPropertyInfo();
            _possibleValueSelectors[propertyInfo] = generator;
            return this;
        }
        public EntroBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IEnumerable<TProperty> sequence)
        {
            var result = Property(propertyExpression, new SequenceGenerator<TProperty>(sequence));
            return result;
        }

        public T Build(TryGet<PropertyInfo, object> tryBuildScalar = null)
        {
            tryBuildScalar = tryBuildScalar ?? TryBuildRandomValueImpl;
            var item = BuildImpl(tryBuildScalar);
            return item;
        }
        public IEnumerable<T> Take(int count)
        {
            var result = Take(count, Seed);
            return result;
        }
        public IEnumerable<T> Take(int count, int seed)
        {
            _random = new Random(seed);
            for (int i = 0; i < count; i++)
            {
                yield return Build();
            }
        }

        T BuildImpl(TryGet<PropertyInfo, object> tryBuildScalar)
        {
            return (T)BuildImpl(typeof(T), tryBuildScalar);
        }
        object BuildImpl(Type type, TryGet<PropertyInfo, object> tryBuildScalar)
        {
            object instance = Activator.CreateInstance(type);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                object value;

                var propertyType = property.PropertyType;
                if (property.GetSetMethod(true) != null)
                {
                    IGenerator generator;
                    if (_possibleValueSelectors.TryGetValue(property, out generator))
                    {
                        value = generator.Next(_random);
                    }
                    else if (propertyType.IsScalar())
                    {
                        tryBuildScalar(property, out value);
                    }
                    else if (propertyType.IsSequence())
                    {
                        value = BuildCollectionImpl(propertyType, tryBuildScalar);
                    }
                    else if (propertyType.IsArray)
                    {
                        // Ignore this for now
                        value = null;
                    }
                    else
                    {
                        value = BuildImpl(propertyType, tryBuildScalar);
                    }

                    property.SetValue(instance, value);
                }
            }

            return instance;
        }
        object BuildCollectionImpl(Type propertyType, TryGet<PropertyInfo, object> tryBuildScalar)
        {
            Type itemType = propertyType.GenericTypeArguments.Single();
            Type collectionType;
            if (propertyType.IsInterface)
            {
                collectionType = typeof(List<>).MakeGenericType(itemType);
            }
            else
            {
                collectionType = propertyType;
            }
            var instance = (IList)Activator.CreateInstance(collectionType);
            var item = BuildImpl(itemType, tryBuildScalar);
            instance.Add(item);
            return instance;
        }
        bool TryBuildRandomValueImpl(PropertyInfo property, out object result)
        {
            Type propertyType = property.PropertyType;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyType.GenericTypeArguments.Single();
                if (_random.Next(0, 2) == 1)
                {
                    result = null;
                    return true;
                }
            }

            if (propertyType == typeof(string))
            {
                result = Guid.NewGuid().ToString().Split('-')[0];
            }
            else if (propertyType == typeof(int))
            {
                result = _random.Next(int.MaxValue);
            }
            else if (propertyType == typeof(long))
            {
                result = (long)(_random.NextDouble() * long.MaxValue);
            }
            else if (propertyType == typeof(decimal))
            {
                result = (decimal)_random.NextDouble() * int.MaxValue;
            }
            else if (propertyType == typeof(double))
            {
                result = _random.NextDouble() * int.MaxValue;
            }
            else if (propertyType == typeof(byte))
            {
                result = (byte)_random.Next(256);
            }
            else if (propertyType == typeof(DateTime))
            {
                result = new DateTime((long)(_random.NextDouble() * DateTime.MaxValue.Ticks));
            }
            else if (propertyType == typeof(Guid))
            {
                result = Guid.NewGuid();
            }
            else if (propertyType == typeof(bool))
            {
                result = _random.Next(0, 2) == 1;
            }
            else
            {
                result = null;
                return false;
            }

            return true;
        }
    }

    public static class EntroBuilder
    {
        public static EntroBuilder<T> Create<T>() where T : class, new()
        {
            var builder = new EntroBuilder<T>();
            return builder;
        }

        public static T Build<T>() where T : class, new()
        {
            var instance = new EntroBuilder<T>().Build();
            return instance;
        }
        public static T Build<T>(TryGet<PropertyInfo, object> tryBuildScalar) where T : class, new()
        {
            var instance = new EntroBuilder<T>().Build(tryBuildScalar);
            return instance;
        }
        public static T Build<T>(int seed, TryGet<PropertyInfo, object> tryBuildScalar) where T : class, new()
        {
            var instance = new EntroBuilder<T>(seed).Build(tryBuildScalar);
            return instance;
        }
    }
}
