using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Netcode
{
    /// <summary> Code by sucss & Feldy @ PellucidMod </summary>
    public static class NetExtensions
    {
        /// <summary>
        /// Returns the <see cref="MemberInfo"/> of every field or property of <c>this</c> that has the <see cref="NetSyncAttribute"/>.
        /// </summary>
        public static MemberInfo[] GetNetSyncMembers(this object obj)
        {
            Type type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             .Where(f => f.GetCustomAttribute<NetSyncAttribute>() is not null);

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .Where(p => p.GetCustomAttribute<NetSyncAttribute>() is not null && p.CanRead && p.CanWrite);

            return fields.Concat<MemberInfo>(properties).OrderBy(m => m.Name).ToArray();
        }

        /// <summary>
        /// Writes all the fields & properties of <c>this</c> that have the <see cref="NetSyncAttribute"/> to the <paramref name="binaryWriter"/>. 
        /// If the <paramref name="bitWriter"/> is not null, boolean fields are written to it instead.
        /// </summary>
        /// <returns><c>true</c> if all fields were written succesfully else <c>false</c>.</returns>
        public static bool NetWrite(this object obj, BinaryWriter binaryWriter, BitWriter bitWriter = null)
        {
            MemberInfo[] netSyncMembers = obj.GetNetSyncMembers();
            if (netSyncMembers.Length == 0)
            {
                return false;
            }

            foreach (var member in netSyncMembers)
            {
                object value;
                Type memberType;

                if (member is FieldInfo fieldInfo)
                {
                    value = fieldInfo.GetValue(obj);
                    memberType = fieldInfo.FieldType;
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    value = propertyInfo.GetValue(obj);
                    memberType = propertyInfo.PropertyType;
                }
                else
                {
                    continue;
                }

                if (memberType.IsEnum)
                    memberType = Enum.GetUnderlyingType(memberType);

                if (memberType == typeof(Vector2))
                {
                    binaryWriter.WriteVector2((Vector2)value);
                }
                else if (memberType == typeof(Point))
                {
                    binaryWriter.WritePoint((Point)value);
                }
                else if (memberType == typeof(Point16))
                {
                    binaryWriter.WritePoint16((Point16)value);
                }
                else if (memberType == typeof(Rectangle))
                {
                    binaryWriter.WriteRectangle((Rectangle)value);
                }
                else if (memberType == typeof(Color))
                {
                    binaryWriter.WriteColor((Color)value);
                }
                else if (memberType == typeof(bool) && bitWriter is not null)
                {
                    bitWriter.WriteBit((bool)value);
                }
                else
                {
                    MethodInfo writeMethod = typeof(BinaryWriter).GetMethod("Write", [memberType]);
                    if (writeMethod is not null)
                    {
                        writeMethod.Invoke(binaryWriter, [value]);
                    }
                    else
                    {
                        Macrocosm.Instance.Logger.Warn($"Couldn't write NetSync member \"{member.Name}\" value with type <{memberType.Name}>.");
                        ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral($"Couldn't write NetSync member \"{member.Name}\" value with type <{memberType.Name}>."), Color.Red);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Reads all the fields of <c>this</c> that have the <see cref="NetSyncAttribute"/> from the <see cref="BinaryReader"/>. 
        /// If the <see cref="BitReader"/> is not null, boolean fields are read from it instead.
        /// </summary>
        /// <param name="reader"></param>
        public static void NetRead(this object obj, BinaryReader binaryReader, BitReader bitReader = null)
        {
            foreach (var member in obj.GetNetSyncMembers())
            {
                Type memberType;
                object value = null;

                if (member is FieldInfo fieldInfo)
                {
                    memberType = fieldInfo.FieldType;
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    memberType = propertyInfo.PropertyType;
                }
                else
                {
                    continue;
                }

                if (memberType.IsEnum)
                    memberType = Enum.GetUnderlyingType(memberType);

                if (memberType == typeof(Vector2))
                {
                    value = binaryReader.ReadVector2();
                }
                else if (memberType == typeof(Point))
                {
                    value = binaryReader.ReadPoint();
                }
                else if (memberType == typeof(Point16))
                {
                    value = binaryReader.ReadPoint16();
                }
                else if (memberType == typeof(Rectangle))
                {
                    value = binaryReader.ReadRectangle();
                }
                else if (memberType == typeof(Color))
                {
                    value = binaryReader.ReadColor();
                }
                else if (memberType == typeof(bool) && bitReader is not null)
                {
                    value = bitReader.ReadBit();
                }
                else
                {
                    MethodInfo readMethod = typeof(BinaryReader).GetMethod($"Read{memberType.Name}");
                    if (readMethod is not null)
                    {
                        value = readMethod.Invoke(binaryReader, null);
                    }
                }

                if (member is FieldInfo field)
                {
                    field.SetValue(obj, value);
                }
                else if (member is PropertyInfo property && property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
            }
        }

        public static void WritePoint(this BinaryWriter writer, Point point)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }

        public static Point ReadPoint(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            return new Point(x, y);
        }

        public static void WritePoint16(this BinaryWriter writer, Point16 point)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }

        public static Point16 ReadPoint16(this BinaryReader reader)
        {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            return new Point16(x, y);
        }

        public static void WriteRectangle(this BinaryWriter writer, Rectangle rectangle)
        {
            writer.Write(rectangle.X);
            writer.Write(rectangle.Y);
            writer.Write(rectangle.Width);
            writer.Write(rectangle.Height);
        }

        public static Rectangle ReadRectangle(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int w = reader.ReadInt32();
            int h = reader.ReadInt32();
            return new Rectangle(x, y, w, h);
        }

        public static void WriteColor(this BinaryWriter writer, Color color)
        {
            writer.Write(color.PackedValue);
        }

        public static Color ReadColor(this BinaryReader reader)
        {
            return new Color() { PackedValue = reader.ReadUInt32() };
        }
    }
}
