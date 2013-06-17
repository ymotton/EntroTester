using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EntroTester
{
    internal static class TypeExtensions
    {
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

        public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var memberExpression = (MemberExpression)((LambdaExpression)propertyExpression).Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return propertyInfo;
        }

        readonly static MethodInfo SelectMethodInfo = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "Select");
        public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, IEnumerable<TProperty>>> propertyExpression)
        {
            var methodCallExpression = (MethodCallExpression)((LambdaExpression)propertyExpression).Body;
            if (methodCallExpression.Method.GetGenericMethodDefinition() == SelectMethodInfo)
            {
                var lambdaExpression = (LambdaExpression)methodCallExpression.Arguments[1];
                var memberExpression = (MemberExpression)lambdaExpression.Body;
                var propertyInfo = (PropertyInfo)memberExpression.Member;
                return propertyInfo;
            }
            return null;
        }

        public static string GetPropertyPath<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var memberExpression = (MemberExpression)((LambdaExpression)propertyExpression).Body;
            var path = memberExpression.ToString();
            int firstDotOffset = path.IndexOf('.');
            if (firstDotOffset == -1)
                return path;
            path = path.Substring(firstDotOffset);
            return typeof(T).Name + path;
        }
        public static string GetPropertyPath<T, TProperty>(this Expression<Func<T, IEnumerable<TProperty>>> propertyExpression)
        {
            var methodCallExpression = (MethodCallExpression)((LambdaExpression)propertyExpression).Body;
            if (methodCallExpression.Method.GetGenericMethodDefinition() != SelectMethodInfo)
            {
                var message = string.Format("Unspported Method '{0}' in expression", methodCallExpression.Method);
                throw new InvalidOperationException(message);
            }
            var path = methodCallExpression.ToString();
            int firstDotOffset = path.IndexOf('.');
            if (firstDotOffset == -1)
                return path;
            path = path.Substring(firstDotOffset);
            var methodsToRemove = new Regex(@"Select\(\w+ \=\> \w+\.").Matches(path).OfType<Match>().Select(ma => ma.Value).ToList();
            foreach (var method in methodsToRemove)
            {
                path = path.Replace(method, "");
            }
            path = path.Replace(")", "");
            return typeof(T).Name + path;
        }
    }
}
