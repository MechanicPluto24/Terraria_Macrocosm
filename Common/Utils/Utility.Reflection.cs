using System;
using System.Collections.Generic;
using System.Reflection;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> fieldCache = new();
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> propertyCache = new();
        private static readonly Dictionary<Type, Dictionary<string, MethodInfo>> methodCache = new();

        /// <summary>
        /// Gets the value of a field using reflection. Caches the FieldInfo. Targets nonpublic fields by default.
        /// </summary>
        /// <param name="type">The type containing the field.</param>
        /// <param name="fieldName">The name of the field to retrieve.</param>
        /// <param name="obj">The instance of the object for instance fields; null for static fields.</param>
        /// <param name="bindingFlags">
        /// Optional binding flags to use. If null, defaults to:
        /// <c>BindingFlags.Instance</c> for instance fields, <c>BindingFlags.Static</c> for static fields, 
        /// and <c>BindingFlags.NonPublic</c>, which <b>excludes public fields</b> by default.
        /// </param>
        /// <returns>The field value, or null if the field does not exist.</returns>
        public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? bindingFlags = null)
            => GetCachedFieldInfo(type, fieldName, bindingFlags ?? GetDefaultBindingFlags(obj))?.GetValue(obj);

        /// <summary>
        /// Sets the value of a field using reflection. Caches the FieldInfo. Targets non-public fields by default.
        /// </summary>
        /// <inheritdoc cref="GetFieldValue"/>
        /// <param name="value">The new value to assign to the field.</param>
        public static void SetFieldValue(this Type type, string fieldName, object value, object obj = null, BindingFlags? bindingFlags = null)
            => GetCachedFieldInfo(type, fieldName, bindingFlags ?? GetDefaultBindingFlags(obj))?.SetValue(obj, value);

        /// <summary>
        /// Gets the value of a field and casts it to the specified type. Caches the FieldInfo. Targets non-public fields by default.
        /// </summary>
        /// <inheritdoc cref="GetFieldValue"/>
        /// <typeparam name="T">The expected return type.</typeparam>
        public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? bindingFlags = null)
            => (T)(GetCachedFieldInfo(type, fieldName, bindingFlags ?? GetDefaultBindingFlags(obj))?.GetValue(obj) ?? default(T));

        /// <summary>
        /// Gets the value of a property using reflection. Caches the PropertyInfo. 
        /// </summary>
        /// <inheritdoc cref="GetFieldValue"/>
        public static object GetPropertyValue(this Type type, string propertyName, object obj = null, BindingFlags? bindingFlags = null)
            => GetCachedPropertyInfo(type, propertyName, bindingFlags ?? GetDefaultBindingFlags(obj))?.GetValue(obj);

        /// <summary>
        /// Gets the value of a property and casts it to the specified type. Caches the PropertyInfo. 
        /// </summary>
        /// <inheritdoc cref="GetPropertyValue"/>
        /// <typeparam name="T">The expected return type.</typeparam>
        public static T GetPropertyValue<T>(this Type type, string propertyName, object obj = null, BindingFlags? bindingFlags = null)
            => (T)(GetCachedPropertyInfo(type, propertyName, bindingFlags ?? GetDefaultBindingFlags(obj))?.GetValue(obj) ?? default(T));

        /// <summary>
        /// Sets the value of a property using reflection. Caches the PropertyInfo. 
        /// </summary>
        /// <inheritdoc cref="GetPropertyValue"/>
        /// <param name="value">The new value to assign to the property.</param>
        public static void SetPropertyValue(this Type type, string propertyName, object value, object obj = null, BindingFlags? bindingFlags = null)
            => GetCachedPropertyInfo(type, propertyName, bindingFlags ?? GetDefaultBindingFlags(obj))?.SetValue(obj, value);

        /// <summary>
        /// Invokes a method using reflection. Caches the MethodInfo.
        /// </summary>
        /// <param name="type">The type containing the method.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="obj">The instance of the object for instance methods; null for static methods.</param>
        /// <param name="bindingFlags">
        /// Optional binding flags to use. If null, defaults to:
        /// <c>BindingFlags.Instance</c> for instance methods, <c>BindingFlags.Static</c> for static methods, 
        /// and <c>BindingFlags.NonPublic</c>, which <b>excludes public methods</b> by default.
        /// </param>
        /// <param name="parameters">Optional parameters for the method.</param>
        /// <returns>The result of the method invocation, or null if the method does not exist. </returns>
        public static object InvokeMethod(this Type type, string methodName, object obj = null, BindingFlags? bindingFlags = null, object[] parameters = null)
            => GetCachedMethodInfo(type, methodName, bindingFlags ?? GetDefaultBindingFlags(obj))?.Invoke(obj, parameters);

        /// <summary>
        /// Invokes a method and casts the return value to the specified type. Caches the MethodInfo. 
        /// </summary>
        /// <typeparam name="TResult">The expected return type.</typeparam>
        /// <returns>The result of the method invocation, or <c>default(<typeparamref name="TResult"/>)</c> if the method does not exist. </returns>
        /// <inheritdoc cref="InvokeMethod"/>
        public static TResult InvokeMethod<TResult>(this Type type, string methodName, object obj = null, BindingFlags? bindingFlags = null, object[] parameters = null)
            => (TResult)(GetCachedMethodInfo(type, methodName, bindingFlags ?? GetDefaultBindingFlags(obj))?.Invoke(obj, parameters) ?? default(TResult));

        private static BindingFlags GetDefaultBindingFlags(object obj)
            => (obj == null ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.NonPublic | BindingFlags.Public;

        private static FieldInfo GetCachedFieldInfo(Type type, string fieldName, BindingFlags flags)
        {
            if (!fieldCache.TryGetValue(type, out var fields))
            {
                fields = new Dictionary<string, FieldInfo>();
                fieldCache[type] = fields;
            }

            if (!fields.TryGetValue(fieldName, out var fieldInfo))
            {
                fieldInfo = type.GetField(fieldName, flags);
                if (fieldInfo != null)
                    fields[fieldName] = fieldInfo;
            }

            return fieldInfo;
        }

        private static PropertyInfo GetCachedPropertyInfo(Type type, string propertyName, BindingFlags flags)
        {
            if (!propertyCache.TryGetValue(type, out var properties))
            {
                properties = new Dictionary<string, PropertyInfo>();
                propertyCache[type] = properties;
            }

            if (!properties.TryGetValue(propertyName, out var propertyInfo))
            {
                propertyInfo = type.GetProperty(propertyName, flags);
                if (propertyInfo != null)
                    properties[propertyName] = propertyInfo;
            }

            return propertyInfo;
        }

        private static MethodInfo GetCachedMethodInfo(Type type, string methodName, BindingFlags flags)
        {
            if (!methodCache.TryGetValue(type, out var methods))
            {
                methods = new Dictionary<string, MethodInfo>();
                methodCache[type] = methods;
            }

            if (!methods.TryGetValue(methodName, out var methodInfo))
            {
                methodInfo = type.GetMethod(methodName, flags);
                if (methodInfo != null)
                    methods[methodName] = methodInfo;
            }

            return methodInfo;
        }

    }
}
