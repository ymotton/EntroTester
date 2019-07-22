using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EntroBuilder
{
    internal static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static bool IsSealed(this Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }

        public static bool IsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;

        }

        public static bool IsPrimitive(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;

        }

        public static bool IsPublic(this Type type)
        {
            return type.GetTypeInfo().IsPublic;

        }

        public static bool IsNestedPublic(this Type type)
        {
            return type.GetTypeInfo().IsNestedPublic;

        }

        public static bool IsFromLocalAssembly(this Type type)
        {
            string assemblyName = type.GetAssembly().GetName().Name;


            try
            {
                Assembly.Load(new AssemblyName { Name = assemblyName });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;

        }

        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;

        }

        public static Type BaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;

        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;

        }

        public static T GetPropertyValue<T>(this Type type, string propertyName, object target)
        {
            PropertyInfo property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            return (T)property.GetValue(target);

        }

        public static void SetPropertyValue(this Type type, string propertyName, object target, object value)
        {
            PropertyInfo property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            property.SetValue(target, value);

        }

        public static T GetFieldValue<T>(this Type type, string fieldName, object target)
        {
            FieldInfo field = type.GetTypeInfo().GetDeclaredField(fieldName);
            return (T)field.GetValue(target);

        }

        public static void SetFieldValue(this Type type, string fieldName, object target, object value)
        {
            FieldInfo field = type.GetTypeInfo().GetDeclaredField(fieldName);
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                type.SetPropertyValue(fieldName, target, value);
            }

        }

        public static void InvokeMethod<T>(this Type type, string methodName, object target, T value)
        {
            MethodInfo method = type.GetTypeInfo().GetDeclaredMethod(methodName);
            method.Invoke(target, new object[] { value });

        }

        public static IEnumerable<MethodInfo> GetMethods(this Type someType)
        {
            var t = someType;
            while (t != null)
            {
                var ti = t.GetTypeInfo();
                foreach (var m in ti.DeclaredMethods)
                    yield return m;
                t = ti.BaseType;
            }
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static bool IsSubclassOf(this Type type, Type c)
        {
            return type.GetTypeInfo().IsSubclassOf(c);
        }

        public static Attribute[] GetCustomAttributes(this Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes().ToArray();
        }

        public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).Cast<Attribute>().ToArray();
        }

        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().GetInterfaces();
        }
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(@interface => @interface == interfaceType);
        }
        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.IsGenericType()
                && (type.GetGenericTypeDefinition() == interfaceType
                 || type.GetInterfaces().Any(@interface => @interface.IsGenericType() && @interface.GetGenericTypeDefinition() == interfaceType));
        }

        public static bool IsScalar(this Type type)
        {
            return type.IsValueType() || type == typeof(string);
        }
        public static bool IsSequence(this Type type)
        {
            bool isSequence = type.ImplementsGenericInterface(typeof(IEnumerable<>));
            return isSequence;
        }
        public static bool IsDictionary(this Type type)
        {
            bool isDictionary = type.ImplementsGenericInterface(typeof (IDictionary<,>));
            return isDictionary;
        }
        public static MethodInfo GetExplicitInterfaceMethod(this Type type, Type interfaceType, string method)
        {
            return  type.GetTypeInfo()
                .GetRuntimeInterfaceMap(interfaceType)
                .TargetMethods
                .SingleOrDefault(x => x.Name == method);
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }
        public static bool IsAssignableFrom(this Type type, Type fromType)
        {
            return type.GetTypeInfo().IsAssignableFrom(fromType);
        }
        public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return propertyInfo;
        }

        static MethodInfo GetMethodFromExpression<T>(Expression<Func<object, T>> expression)
        {
            if (expression.Body is MethodCallExpression methodCallExpression)
            {
                return methodCallExpression.Method.GetGenericMethodDefinition();
            }
            throw new NotSupportedException("Supported method call expressions are Enumerable.Select(IE<TSource> source, x => x) and Enumerable.SelectMany(IE<TSource> source, x => Enumerable.Empty<TResult>())");
        }
        static readonly MethodInfo SelectMethodInfo = GetMethodFromExpression(_ => Enumerable.Empty<object>().Select(x => x));
        static readonly MethodInfo SelectManyMethodInfo = GetMethodFromExpression(_ => Enumerable.Empty<object>().SelectMany(x => Enumerable.Empty<object>()));
        public static string GetPropertyPath<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var lambdaBodyExpression = propertyExpression.Body;

            string path;

            var memberExpression = lambdaBodyExpression as MemberExpression;
            if (memberExpression != null)
            {
                path = memberExpression.ToString();
                int firstDotOffset = path.IndexOf('.');
                if (firstDotOffset == -1)
                    return path;
                path = path.Substring(firstDotOffset);
            }
            else
            {
                // Only accept Select at the end of the expression
                // SelectMany would not make much sense
                var methodCallExpression = (MethodCallExpression)lambdaBodyExpression;
                var methodDefinition = methodCallExpression.Method.GetGenericMethodDefinition();
                if (methodDefinition != SelectMethodInfo && methodDefinition != SelectManyMethodInfo)
                {
                    var message = string.Format("Unsupported Method '{0}' in expression", methodCallExpression.Method);
                    throw new InvalidOperationException(message);
                }
                
                // Truncate first section
                path = methodCallExpression.ToString();
                int firstDotOffset = path.IndexOf('.');
                if (firstDotOffset == -1)
                    return path;
                path = path.Substring(firstDotOffset);

                // Removes all 'Select(x => x' sections
                var methodsToRemove = new Regex(@"Select\(\w+ \=\> \w+\.").Matches(path).OfType<Match>().Select(ma => ma.Value).ToList();
                foreach (var method in methodsToRemove)
                {
                    path = path.Replace(method, "");
                }
                // Removes all 'SelectMany(x => x.' sections
                methodsToRemove = new Regex(@"SelectMany\(\w+ \=\> \w+\.").Matches(path).OfType<Match>().Select(ma => ma.Value).ToList();
                foreach (var method in methodsToRemove)
                {
                    path = path.Replace(method, "");
                }
                // Removes all ending paranthesis
                path = path.Replace(")", "");
            }
            
            return typeof(T).Name + path;
        }
    }
}
