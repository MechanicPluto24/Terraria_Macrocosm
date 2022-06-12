using System;
using System.Reflection;

namespace Macrocosm.Common.Utility {
    public static class ReflectionHelper {
        public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null) {
            if (flags == null) {
                flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            }
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return field.GetValue(obj);
        }

        public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null) {
            if (flags == null) {
                flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            }
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return (T)field.GetValue(obj);
        }
    }
}