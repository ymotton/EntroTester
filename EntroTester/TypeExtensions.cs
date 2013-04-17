using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
    }
}
