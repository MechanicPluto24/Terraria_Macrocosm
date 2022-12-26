using System;
using System.Linq;
using System.Reflection;

namespace Macrocosm.Common.Utility
{
	public static class ReflectionHelper
	{
		public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
		{
			if (flags == null)
			{
				flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			}
			FieldInfo field = type.GetField(fieldName, flags.Value);
			return field.GetValue(obj);
		}

		public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
		{
			if (flags == null)
			{
				flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			}
			FieldInfo field = type.GetField(fieldName, flags.Value);
			return (T)field.GetValue(obj);
		}

		public static Type[] GetImplementedInterfaces(this Type type, Type interfaceType)   
			=> type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType).ToArray();

		// i have no idea what i am doing 
		public static Type GetGenericOfInterface(this Type type, Type interfaceType, int interfaceIndex = 0, int genericTypeIndex = 0)
		{
			Type[] types = type.GetImplementedInterfaces(interfaceType);
			return types[interfaceIndex].GetGenericArguments()[genericTypeIndex];
		}



	}
}