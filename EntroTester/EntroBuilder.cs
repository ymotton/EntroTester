﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EntroTester
{
    public class EntroBuilder<T>  where T : class, new()
    {
        const int Seed = 0x1;
        Random _random;

        public EntroBuilder(int? seed = null)
        {
            _random = new Random(seed ?? Seed);
        }

        readonly Dictionary<string, IGenerator> _possibleValueSelectors = new Dictionary<string, IGenerator>();
        public EntroBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IGenerator<TProperty> generator)
        {
            var path = propertyExpression.GetPropertyPath();
            _possibleValueSelectors[path] = generator;
            return this;
        }
        public EntroBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IEnumerable<TProperty> sequence)
        {
            var result = Property(propertyExpression, new SequenceGenerator<TProperty>(sequence));
            return result;
        }

        public EntroBuilder<T> Property<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> propertyExpression, IGenerator<TProperty> generator)
        {
            var path = propertyExpression.GetPropertyPath();
            _possibleValueSelectors[path] = generator;
            return this;
        }
        public EntroBuilder<T> Property<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> propertyExpression, IEnumerable<TProperty> sequence)
        {
            var result = Property(propertyExpression, new SequenceGenerator<TProperty>(sequence));
            return result;
        }

        public T Build()
        {
            var item = BuildImpl();
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

        T BuildImpl()
        {
            var context = new TypeContext(typeof(T));
            return (T)BuildImpl(context, typeof(T));
        }
        object BuildImpl(TypeContext context, Type type)
        {
            object instance = Activator.CreateInstance(type);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                object value;

                var propertyContext = context.AddProperty(property);
                var propertyType = property.PropertyType;
                if (property.GetSetMethod(true) != null)
                {
                    IGenerator generator;
                    if (_possibleValueSelectors.TryGetValue(propertyContext.ToString(), out generator))
                    {
                        if (propertyType.IsSequence())
                        {
                            value = BuildCollectionForGeneratorImpl(propertyType, generator);
                        }
                        else
                        {
                            value = generator.Next(_random);
                        }
                    }
                    else if (propertyType.IsScalar())
                    {
                        TryBuildRandomValueImpl(property, out value);
                    }
                    else if (propertyType.IsSequence())
                    {
                        value = BuildCollectionImpl(propertyContext, propertyType);
                    }
                    else if (propertyType.IsArray)
                    {
                        // Ignore this for now
                        value = null;
                    }
                    else
                    {
                        value = BuildImpl(propertyContext, propertyType);
                    }

                    property.SetValue(instance, value, new object[0]);
                }
            }

            return instance;
        }
        object BuildCollectionImpl(TypeContext context, Type propertyType)
        {
            Type elementType = propertyType.GetGenericArguments().Single();
            Type collectionType;
            if (propertyType.IsInterface)
            {
                collectionType = typeof(List<>).MakeGenericType(elementType);
            }
            else
            {
                collectionType = propertyType;
            }
            var instance = (IList)Activator.CreateInstance(collectionType);
            var item = BuildImpl(context, elementType);
            instance.Add(item);
            return instance;
        }
        object BuildCollectionForGeneratorImpl(Type propertyType, IGenerator generator)
        {
            object instance;

            var elementType = propertyType.GetGenericArguments().Single();
            var generatorType = generator.GetType();
            var interfaceGeneratorType = typeof(IGenerator<>).MakeGenericType(typeof(List<>).MakeGenericType(elementType));
            if (generatorType.ImplementsInterface(interfaceGeneratorType))
            {
                var sequence = generator.Next(_random);
                if (sequence.GetType() != propertyType)
                {
                    instance = Activator.CreateInstance(propertyType, sequence);
                }
                else
                {
                    instance = sequence;
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            return instance;
        }
        bool TryBuildRandomValueImpl(PropertyInfo property, out object result)
        {
            Type propertyType = property.PropertyType;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyType.GetGenericArguments().Single();
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

        class TypeContext
        {
            readonly string _baseType;
            readonly List<string> _members;
            public TypeContext(Type type)
            {
                _baseType = type.Name;
                _members = new List<string>();
            }
            private TypeContext(TypeContext context, PropertyInfo propertyInfo)
            {
                _baseType = context._baseType;
                _members = new List<string>(context._members);
                _members.Add(propertyInfo.Name);
            }
            public TypeContext AddProperty(PropertyInfo propertyInfo)
            {
                return new TypeContext(this, propertyInfo);
            }
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(_baseType);
                foreach (var member in _members)
                {
                    sb.AppendFormat(".{0}", member);
                }
                return sb.ToString();
            }
        }
    }

    public static class EntroBuilder
    {
        public static EntroBuilder<T> Create<T>() where T : class, new()
        {
            var builder = new EntroBuilder<T>();
            return builder;
        }
        public static EntroBuilder<T> Create<T>(int seed) where T : class, new()
        {
            var builder = new EntroBuilder<T>(seed);
            return builder;
        }

        public static T Build<T>() where T : class, new()
        {
            var instance = new EntroBuilder<T>().Build();
            return instance;
        }
        public static T Build<T>(int seed) where T : class, new()
        {
            var instance = new EntroBuilder<T>(seed).Build();
            return instance;
        }
    }
}
