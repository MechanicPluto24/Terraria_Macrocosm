using System;
using System.Reflection;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return field.GetValue(obj);
        }

        public static void SetFieldValue(this Type type, string fieldName, object value, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo field = type.GetField(fieldName, flags.Value);
            field?.SetValue(obj, value);
        }

        public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return (T)field.GetValue(obj);
        }

        public static object GetPropertyValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            PropertyInfo property = type.GetProperty(fieldName, flags.Value);
            return property.GetValue(obj);
        }

        public static void SetPropertyValue(this Type type, string fieldName, object value, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            PropertyInfo property = type.GetProperty(fieldName, flags.Value);
            property?.SetValue(obj, value);
        }

        public static T GetPropertyValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            PropertyInfo property = type.GetProperty(fieldName, flags.Value);
            return (T)property.GetValue(obj);
        }

        public static object InvokeStaticMethod(this Type type, string methodName, params object[] parameters)
            => InvokeMethod(type, methodName, parameters);

        /// <summary> Invokes a private method on a given object. </summary>
        /// <param name="instance"> The object on which to invoke the method. If the method is static, pass the type. </param>
        /// <param name="methodName"> Name of the method to invoke. </param>
        /// <param name="parameters"> Parameters to pass to the method. </param>
        /// <returns> Result of the method invocation. </returns>
        public static object InvokeMethod(object instance, string methodName, params object[] parameters)
        {
            Type type = instance as Type ?? instance.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            return methodInfo == null
                ? throw new ArgumentException($"Method '{methodName}' not found on type {type.FullName}.", nameof(methodName))
                : methodInfo.Invoke(instance is Type ? null : instance, parameters);
        }
    }
}