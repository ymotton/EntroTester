using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace EntroBuilder
{
    internal static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(@interface => @interface == interfaceType);
        }
        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.IsGenericType
                && (type.GetGenericTypeDefinition() == interfaceType
                 || type.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceType));
        }

        public static bool IsScalar(this Type type)
        {
            return type.IsValueType || type == typeof(string);
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
            return  type.GetInterfaceMap(interfaceType)
                .TargetMethods
                .SingleOrDefault(x => x.Name == method);
        }

        public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var memberExpression = (MemberExpression)((LambdaExpression)propertyExpression).Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return propertyInfo;
        }
        readonly static MethodInfo SelectMethodInfo = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "Select");
        readonly static MethodInfo SelectManyMethodInfo = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "SelectMany");
        public static string GetPropertyPath<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var lambdaBodyExpression = ((LambdaExpression)propertyExpression).Body;

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
