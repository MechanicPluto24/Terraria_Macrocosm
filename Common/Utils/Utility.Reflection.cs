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

        public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return (T)field.GetValue(obj);
        }

        /// <summary> Invokes a private method on a given object. </summary>
        /// <param name="instance"> The object on which to invoke the method. If the method is static, pass the type. </param>
        /// <param name="methodName"> Name of the method to invoke. </param>
        /// <param name="parameters"> Parameters to pass to the method. </param>
        /// <returns> Result of the method invocation. </returns>
        public static object InvokeMethod(object instance, string methodName, params object[] parameters)
        {
            if (instance == null && !(parameters?.Length > 0 && parameters[0] is Type))
                throw new ArgumentNullException(nameof(instance), "The instance should not be null, unless you're invoking a static method and the first parameter in 'parameters' is of type 'Type'.");

            Type type = instance as Type ?? instance.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            return methodInfo == null
                ? throw new ArgumentException($"Method '{methodName}' not found on type {type.FullName}.", nameof(methodName))
                : methodInfo.Invoke(instance is Type ? null : instance, parameters);
        }

    }
}