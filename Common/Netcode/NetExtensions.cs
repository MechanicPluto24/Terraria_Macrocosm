using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Chat;

namespace Macrocosm.Common.Netcode
{
	public static class NetExtensions
	{
		/// <summary>
		/// Returns <see cref="FieldInfo"/> of every field of <c>this</c> that has the <see cref="NetSyncAttribute"/>.
		/// </summary>
		public static FieldInfo[] GetNetSyncFields(this object obj) => obj.GetType().GetFields()
									.Where(fieldInfo => fieldInfo.GetCustomAttribute<NetSyncAttribute>() is not null)
									.OrderBy(fieldInfo => fieldInfo.Name).ToArray();

		/// <summary>
		/// Writes all the fields of <c>this</c> that has the <see cref="NetSyncAttribute"/> to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <returns>true if all fields were written succesfully else false.</returns>
		public static bool NetWriteFields(this object obj, BinaryWriter writer)
		{
			foreach (FieldInfo fieldInfo in obj.GetNetSyncFields())
			{
				switch (fieldInfo.FieldType.Name)
				{
					case "Vector2":
						writer.WriteVector2((Vector2)fieldInfo.GetValue(obj));
						break;

					default:
						MethodInfo methodInfo = typeof(BinaryWriter).GetMethod("Write", new Type[] { fieldInfo.FieldType });
						if (methodInfo is not null)
						{
							methodInfo.Invoke(writer, new object[] { fieldInfo.GetValue(obj) });
						}
						else
						{
							Macrocosm.Instance.Logger.Warn(Terraria.Localization.NetworkText.FromLiteral($"{obj.GetType().FullName}: Couldn't write NetSync field \"{fieldInfo.Name}\" value with type <{fieldInfo.FieldType.Name}>."));
							ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral($"{obj.GetType().FullName}: Couldn't write NetSync field \"{fieldInfo.Name}\" value with type <{fieldInfo.FieldType.Name}>."), Color.Red);
							return false;
						}
						break;
				}
			}

			return true;
		}

		/// <summary>
		/// Reads all the fields of <c>this</c> that has the <see cref="NetSyncAttribute"/> from the reader.
		/// </summary>
		/// <param name="reader"></param>
		public static void NetReadFields(this object obj, BinaryReader reader)
		{
			foreach (FieldInfo fieldInfo in obj.GetNetSyncFields())
			{
				switch (fieldInfo.FieldType.Name)
				{
					case "Vector2":
						fieldInfo.SetValue(obj, reader.ReadVector2());
						break;

					default:
						fieldInfo.SetValue(obj, typeof(BinaryReader).GetMethod($"Read{fieldInfo.FieldType.Name}").Invoke(reader, null));
						break;
				}
			}
		}
	}
}
