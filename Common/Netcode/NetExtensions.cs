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
        /// Returns the <see cref="FieldInfo"/> of every field of <c>this</c> that has the <see cref="NetSyncAttribute"/>.
        /// </summary>
        public static FieldInfo[] GetNetSyncFields(this object obj) => obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(fieldInfo => fieldInfo.GetCustomAttribute<NetSyncAttribute>() is not null)
                                    .OrderBy(fieldInfo => fieldInfo.Name).ToArray();

        /// <summary>
        /// Writes all the fields of <c>this</c> that have the <see cref="NetSyncAttribute"/> to the <paramref name="binaryWriter"/>. 
        /// If the <paramref name="bitWriter"/> is not null, boolean fields are written to it instead.
        /// </summary>
        /// <returns><c>true</c> if all fields were written succesfully else <c>false</c>.</returns>
        public static bool NetWriteFields(this object obj, BinaryWriter binaryWriter, BitWriter bitWriter = null)
        {
            FieldInfo[] netSyncFields = obj.GetNetSyncFields();
            if (netSyncFields.Length == 0)
            {
                return false;
            }

            foreach (FieldInfo fieldInfo in netSyncFields)
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsEnum)
                    fieldType = Enum.GetUnderlyingType(fieldInfo.FieldType);

                if (fieldType == typeof(Vector2))
                {
                    binaryWriter.WriteVector2((Vector2)fieldInfo.GetValue(obj));
                }
                else if (fieldType == typeof(Point))
                {
                    binaryWriter.WritePoint((Point)fieldInfo.GetValue(obj));
                }
                else if (fieldType == typeof(Point16))
                {
                    binaryWriter.WritePoint16((Point16)fieldInfo.GetValue(obj));
                }
                else if (fieldType == typeof(Rectangle))
                {
                    binaryWriter.WriteRectangle((Rectangle)fieldInfo.GetValue(obj));
                }
                else if (fieldType == typeof(Color))
                {
                    binaryWriter.WriteColor((Color)fieldInfo.GetValue(obj));
                }
                else if (fieldType == typeof(bool) && bitWriter is not null)
                {
                    bitWriter.WriteBit((bool)fieldInfo.GetValue(obj));
                }
                else
                {
                    MethodInfo methodInfo = typeof(BinaryWriter).GetMethod("Write", [fieldType]);
                    if (methodInfo is not null)
                    {
                        methodInfo.Invoke(binaryWriter, [fieldInfo.GetValue(obj)]);
                    }
                    else
                    {
                        Macrocosm.Instance.Logger.Warn(Terraria.Localization.NetworkText.FromLiteral($"{obj.GetType().FullName}: Couldn't write NetSync field \"{fieldInfo.Name}\" value with type <{fieldInfo.FieldType.Name}>."));
                        ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral($"{obj.GetType().FullName}: Couldn't write NetSync field \"{fieldInfo.Name}\" value with type <{fieldInfo.FieldType.Name}>."), Color.Red);
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
        public static void NetReadFields(this object obj, BinaryReader binaryReader, BitReader bitReader = null)
        {
            foreach (FieldInfo fieldInfo in obj.GetNetSyncFields())
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsEnum)
                    fieldType = Enum.GetUnderlyingType(fieldInfo.FieldType);

                if (fieldType == typeof(Vector2))
                {
                    fieldInfo.SetValue(obj, binaryReader.ReadVector2());
                }
                else if (fieldType == typeof(Point))
                {
                    fieldInfo.SetValue(obj, binaryReader.ReadPoint());
                }
                else if (fieldType == typeof(Point16))
                {
                    fieldInfo.SetValue(obj, binaryReader.ReadPoint16());
                }
                else if (fieldType == typeof(Rectangle))
                {
                    fieldInfo.SetValue(obj, binaryReader.ReadRectangle());
                }
                else if (fieldType == typeof(Color))
                {
                    fieldInfo.SetValue(obj, binaryReader.ReadColor());
                }
                else if (fieldType == typeof(bool) && bitReader is not null)
                {
                    fieldInfo.SetValue(obj, bitReader.ReadBit());
                }
                else
                {  
                    fieldInfo.SetValue(obj, typeof(BinaryReader).GetMethod($"Read{fieldType.Name}").Invoke(binaryReader, null));
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
