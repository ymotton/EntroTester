using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using EntroBuilder.Generators;

namespace EntroBuilder
{
    public class Builder<T>
    {
        const int DefaultSeed = 0x1;
        Random _random;

        public Builder() : this(DefaultSeed) { }
        public Builder(int seed) : this(new Random(seed)) { }
        public Builder(Random random)
        {
            _random = random;
            RegisterDefaultGenerators();
        }

        void RegisterDefaultGenerators()
        {
            For(new CharGenerator());
            For(new BoolGenerator());
            For(new ByteGenerator());
            For(new DateTimeGenerator());
            For(new DecimalGenerator());
            For(new DoubleGenerator());
            For(new FloatGenerator());
            For(new GuidGenerator());
            For(new Int16Generator());
            For(new Int32Generator());
            For(new Int64Generator());
            For(new UInt16Generator());
            For(new UInt32Generator());
            For(new UInt64Generator());
            For(new SByteGenerator());
            For(NullableGenerator.Create(new CharGenerator()));
            For(NullableGenerator.Create(new BoolGenerator()));
            For(NullableGenerator.Create(new ByteGenerator()));
            For(NullableGenerator.Create(new DateTimeGenerator()));
            For(NullableGenerator.Create(new DecimalGenerator()));
            For(NullableGenerator.Create(new DoubleGenerator()));
            For(NullableGenerator.Create(new FloatGenerator()));
            For(NullableGenerator.Create(new GuidGenerator()));
            For(NullableGenerator.Create(new Int16Generator()));
            For(NullableGenerator.Create(new Int32Generator()));
            For(NullableGenerator.Create(new Int64Generator()));
            For(NullableGenerator.Create(new UInt16Generator()));
            For(NullableGenerator.Create(new UInt32Generator()));
            For(NullableGenerator.Create(new UInt64Generator()));
            For(NullableGenerator.Create(new SByteGenerator()));
            For(new StringGenerator());
        }

        readonly Dictionary<Type, IGenerator> _typeGenerators = new Dictionary<Type, IGenerator>();
        readonly Dictionary<Type, Type> _interfaceMap = new Dictionary<Type, Type>();

        public Builder<T> For<TType>(IGenerator<TType> generator)
        {
            return For(typeof(TType), generator);
        }
        public Builder<T> For(Type type, IGenerator generator)
        {
            _typeGenerators[type] = generator;
            return this;
        }

        public Builder<T> For<TInterface, TImplementation>()
        {
            return For(typeof(TInterface), typeof(TImplementation));
        }
        public Builder<T> For(Type interfaceType, Type implementationType)
        {
            if (!interfaceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException($"Argument {nameof(implementationType)} '{implementationType}' should be assignable to {nameof(interfaceType)} '{interfaceType}'.");
            }
            if (!implementationType.IsClass() || implementationType.IsAbstract())
            {
                throw new ArgumentException($"Argument {nameof(implementationType)} '{implementationType}' should a concrete class.");
            }
            _interfaceMap[interfaceType] = implementationType;

            return this;
        }

        readonly Dictionary<string, IGenerator> _propertyGenerators = new Dictionary<string, IGenerator>();
        public Builder<T> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IGenerator<TProperty> generator)
        {
            var path = propertyExpression.GetPropertyPath();
            _propertyGenerators[path] = generator;
            return this;
        }
        public Builder<T> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IEnumerable<TProperty> sequence)
        {
            var result = Property(propertyExpression, new SequenceGenerator<TProperty>(sequence));
            return result;
        }
        public Builder<T> Property<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> propertyExpression, IGenerator<TProperty> generator)
        {
            var path = propertyExpression.GetPropertyPath();
            _propertyGenerators[path] = generator;
            return this;
        }
        public Builder<T> Property<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> propertyExpression, IEnumerable<TProperty> sequence)
        {
            var result = Property(propertyExpression, new SequenceGenerator<TProperty>(sequence));
            return result;
        }

        ListGenerator.Configuration _listGeneratorConfiguration;
        public Builder<T> Configure(ListGenerator.Configuration configuration)
        {
            _listGeneratorConfiguration = configuration;
            return this;
        }
        
        public T Build()
        {
            var context = new TypeContext(_interfaceMap, typeof(T));
            var item = BuildImpl(context);
            return item;
        }
        public IEnumerable<T> Take(int count)
        {
            var context = new TypeContext(_interfaceMap, typeof(T));
            for (int i = 0; i < count; i++)
            {
                yield return BuildImpl(context);
            }
        }

        T BuildImpl(TypeContext context)
        {
            var classInstanceCache = new Dictionary<Type, object>();
            return (T)BuildImpl(context, typeof(T), classInstanceCache);
        }

        object BuildImpl(TypeContext context, Type type, Dictionary<Type, object> classInstanceCache, bool generateNew = false)
        {
            object instance;

            if (_interfaceMap.TryGetValue(type, out Type implementationType))
            {
                type = implementationType;
            }

            IGenerator generator;
            if (_propertyGenerators.TryGetValue(context.ToString(), out generator)
             || _typeGenerators.TryGetValue(type, out generator))
            {
                // If the generator returns a T[] and the property takes a T[]
                var generatorType = generator.GetType();
                var isGeneric = generatorType.ImplementsGenericInterface(typeof(IGenerator<>));

                // If it's a typeless generator, try to match the returnvalue with property
                // This will produce a runtime error, if it cannot be matched
                if (!isGeneric || type.IsAssignableFrom(generatorType.GetGenericArguments()[0]))
                {
                    instance = generator.Next(_random);
                }
                else
                {
                    instance = BuildCollectionForGeneratorImpl(type, generator);
                }
            }
            else if (context.IsDictionary)
            {
                instance = BuildDictionaryImpl(context, type, classInstanceCache);
            }
            else if (context.IsSequence || context.IsArray)
            {
                instance = BuildCollectionImpl(context, type, classInstanceCache);
            }
            else if (context.IsEnum)
            {
                var sequenceGeneratorType = typeof(SequenceGenerator<>).MakeGenericType(type);
                var rawPossibleValues = Enum.GetValues(type);
                var possibleValues = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type).Invoke(null, new object[] { rawPossibleValues });
                generator = (IGenerator)Activator.CreateInstance(sequenceGeneratorType, possibleValues, 1000000);
                For(type, generator);
                instance = generator.Next(_random);
            }
            else if (context.IsValueType)
            {
                // In case the value type is nullable, don't reflect over its private members
                // We only want to set its Value field.
                if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var innerType = type.GetGenericArguments()[0];
                    instance = _random.Next(0, 2) == 1 ? BuildImpl(context, innerType, classInstanceCache) : null;
                }
                else
                {
                    instance = Activator.CreateInstance(type);
                    var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        var typeContext = context.AddField(field);
                        var fieldType = field.FieldType;

                        object value = BuildImpl(typeContext, fieldType, classInstanceCache);

                        field.SetValue(instance, value);
                    }
                }
            }
            else if (context.IsClass)
            {
                // The root object should only served from cache if we are in a nested part of the object graph
                // This cache is used to reduce endless recursion
                if (generateNew || !classInstanceCache.TryGetValue(type, out instance))
                {
                    if (context.IsAbstract)
                    {
                        return null;
                    }

                    instance = Activator.CreateInstance(type, true);
                    classInstanceCache[type] = instance;

                    foreach (var property in context.Properties)
                    {
                        var propertyContext = context.AddProperty(property);
                        var propertyType = property.PropertyType;
                        object value = BuildImpl(propertyContext, propertyType, classInstanceCache);
                        propertyContext.SetValue(instance, value);
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Type {type} @ {context} does not have a built-in generator, and no user-defined generator was provided. See Builder<T>.For<TInterface, TImplementation>().");
            }

            return instance;
        }
        object BuildDictionaryImpl(TypeContext context, Type propertyType, Dictionary<Type, object> classInstanceCache)
        {
            Type keyType = propertyType.GetGenericArguments().First();
            Type valueType = propertyType.GetGenericArguments().Skip(1).Single();
            Type dictionaryType;
            if (propertyType.IsInterface())
            {
                dictionaryType = typeof (Dictionary<,>).MakeGenericType(keyType, valueType);
            }
            else
            {
                dictionaryType = propertyType;
            }
            var dictionaryInstance = (IDictionary)Activator.CreateInstance(dictionaryType);
            var keyInstance = BuildImpl(context, keyType, classInstanceCache);
            var valueInstance = BuildImpl(context, valueType, classInstanceCache);
            dictionaryInstance.Add(keyInstance, valueInstance);
            return dictionaryInstance;
        }
        object BuildCollectionImpl(TypeContext context, Type propertyType, Dictionary<Type, object> classInstanceCache)
        {
            var generator = new ListGenerator(_listGeneratorConfiguration, propertyType, (t, r) => BuildImpl(context.AddSequenceElement(t), t, classInstanceCache, true));
            return generator.Next(_random);
        }
        object BuildCollectionForGeneratorImpl(Type propertyType, IGenerator generator)
        {
            object instance;

            var elementType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().Single();
            var generatorType = generator.GetType();
            var collectionType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var interfaceGeneratorType = typeof(IGenerator<>).MakeGenericType(collectionType);
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
            else if (typeof(IGenerator<>).MakeGenericType(elementType).IsAssignableFrom(generatorType))
            {
                instance = new ListGenerator(_listGeneratorConfiguration, propertyType, (t, r) => generator.Next(r)).Next(_random);
            }
            else
            {
                throw new NotSupportedException();
            }

            return instance;
        }

        class TypeContext
        {
            readonly Dictionary<Type, Type> _interfaceMap;
            readonly string _path;
            readonly Dictionary<MemberInfo, TypeContext> _memberContexts;
            readonly PropertyInfo _propertyInfo;

            public readonly bool IsDictionary;
            public readonly bool IsSequence;
            public readonly bool IsArray;
            public readonly bool IsClass;
            public readonly bool IsEnum;
            public readonly bool IsValueType;
            public readonly bool IsAbstract;
            public readonly PropertyInfo[] Properties;

            public TypeContext(Dictionary<Type, Type> interfaceMap, Type type) 
                : this(interfaceMap, type.Name, type) { }

            TypeContext(TypeContext context, PropertyInfo propertyInfo)
                : this(context._interfaceMap, context._path + "." + propertyInfo.Name, propertyInfo.PropertyType)
            {
                var propertyOnDeclaringType = propertyInfo.DeclaringType?.GetTypeInfo().GetProperty(propertyInfo.Name);
                if (propertyOnDeclaringType == null) return;
                if (propertyOnDeclaringType.GetSetMethod(true) == null) return;
                _propertyInfo = propertyOnDeclaringType;
            }
            TypeContext(TypeContext context, FieldInfo fieldInfo)
                : this(context._interfaceMap, context._path + "." + fieldInfo.Name, fieldInfo.FieldType) { }

            TypeContext(Dictionary<Type, Type> interfaceMap, string path, Type type)
            {
                _interfaceMap = interfaceMap;
                if (interfaceMap.TryGetValue(type, out Type implementationType))
                {
                    type = implementationType;
                }

                _path = path;
                _memberContexts = new Dictionary<MemberInfo, TypeContext>();

                IsDictionary = type.IsDictionary();
                IsSequence = type.IsSequence();
                IsArray = type.IsArray;
                IsClass = type.IsClass();
                IsEnum = type.IsEnum();
                IsValueType = type.IsValueType();
                IsAbstract = type.IsAbstract();
                Properties = type
                    .GetTypeInfo()
                    .GetProperties()
                    .Where(p => p.GetIndexParameters().Length == 0) // Ignore indexed properties
                    .ToArray();
            }

            public bool IsRoot()
            {
                return !_path.Contains(".");
            }
            public TypeContext AddProperty(PropertyInfo propertyInfo)
            {
                if (!_memberContexts.TryGetValue(propertyInfo, out var memberContext))
                {
                    memberContext = new TypeContext(this, propertyInfo);
                    _memberContexts.Add(propertyInfo, memberContext);
                }
                return memberContext;
            } 
            public TypeContext AddField(FieldInfo fieldInfo)
            {
                if (!_memberContexts.TryGetValue(fieldInfo, out var memberContext))
                {
                    memberContext = new TypeContext(this, fieldInfo);
                    _memberContexts.Add(fieldInfo, memberContext);
                }
                return memberContext;
            }

            TypeContext _sequenceElementContext;
            public TypeContext AddSequenceElement(Type type)
            {
                if (_sequenceElementContext == null)
                {
                    _sequenceElementContext = new TypeContext(_interfaceMap, _path, type);
                }
                return _sequenceElementContext;
            }

            public override string ToString()
            {
                return _path;
            }

            public void SetValue(object instance, object value)
            {
                if (_propertyInfo == null) return;
                _propertyInfo.SetValue(instance, value, new object[0]);
            }
        }
    }

    public static class Builder
    {
        public static Builder<T> Create<T>() 
        {
            var builder = new Builder<T>();
            return builder;
        }
        public static Builder<T> Create<T>(Random random)
        {
            var builder = new Builder<T>(random);
            return builder;
        }
        public static Builder<T> Create<T>(int seed) 
        {
            var builder = new Builder<T>(seed);
            return builder;
        }

        public static T Build<T>() 
        {
            var instance = new Builder<T>().Build();
            return instance;
        }
        public static T Build<T>(Random random)
        {
            var instance = new Builder<T>(random).Build();
            return instance;
        }
        public static T Build<T>(int seed) 
        {
            var instance = new Builder<T>(seed).Build();
            return instance;
        }
    }
}
