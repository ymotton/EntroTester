using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EntroBuilder
{
    public class Builder<T>
    {
        const int Seed = 0x1;
        Random _random;

        public Builder(int? seed = null)
        {
            _random = new Random(seed ?? Seed);

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
            For(new SByteGenerator());
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
            For(NullableGenerator.Create(new SByteGenerator()));
            For(new StringGenerator());
        }

        readonly Dictionary<Type, IGenerator> _typeGenerators = new Dictionary<Type, IGenerator>();
        public Builder<T> For<TType>(IGenerator<TType> generator)
        {
            _typeGenerators[typeof(TType)] = generator;
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
            object instance;
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
            else if (type.IsSequence())
            {
                instance = BuildCollectionImpl(context, type);
            }
            else if (type.IsArray)
            {
                return null;
            }
            else if (type.IsClass)
            {
                instance = Activator.CreateInstance(type);

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var typeContext = context.AddProperty(property);
                    var propertyType = property.PropertyType;
                    if (property.GetSetMethod(true) == null) continue;

                    object value = BuildImpl(typeContext, propertyType);

                    property.SetValue(instance, value, new object[0]);
                }
            }
            else if (type.IsValueType)
            {
                instance = Activator.CreateInstance(type);

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var typeContext = context.AddField(field);
                    var fieldType = field.FieldType;

                    object value = BuildImpl(typeContext, fieldType);

                    field.SetValue(instance, value);
                }
            }
            else
            {
                throw new NotSupportedException();
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
            var interfaceGeneratorType = typeof(IGenerator<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(elementType));
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
            private TypeContext(TypeContext context, FieldInfo fieldInfo)
            {
                _baseType = context._baseType;
                _members = new List<string>(context._members);
                _members.Add(fieldInfo.Name);
            }
            public TypeContext AddProperty(PropertyInfo propertyInfo)
            {
                return new TypeContext(this, propertyInfo);
            } 
            public TypeContext AddField(FieldInfo fieldInfo)
            {
                return new TypeContext(this, fieldInfo);
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

    public static class Builder
    {
        public static Builder<T> Create<T>() 
        {
            var builder = new Builder<T>();
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
        public static T Build<T>(int seed) 
        {
            var instance = new Builder<T>(seed).Build();
            return instance;
        }
    }
}
