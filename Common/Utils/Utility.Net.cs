using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static void SendData(int dataType, int dataA, int dataB, string text, int playerID, float dataC, float dataD, float dataE, int clientType)
        {
            NetMessage.SendData(dataType, dataA, dataB, NetworkText.FromLiteral(text), playerID, dataC, dataD, dataE, clientType);
        }

        public static byte[] GetBuffer(this ModPacket packet)
        {
            //var len = (ushort)packet.BaseStream.Position;
            //packet.Seek(0, SeekOrigin.Begin);
            //packet.Write(len);
            return ((MemoryStream)packet.BaseStream).GetBuffer();
        }

        public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
        {
            packet.Write((byte)msg);
            for (int m = 0; m < param.Length; m++)
            {
                object obj = param[m];

                if (obj is byte[])
                {
                    byte[] array = (byte[])obj;
                    foreach (byte b in array) packet.Write((byte)b);
                }
                else
                if (obj is bool) packet.Write((bool)obj);
                else
                if (obj is byte) packet.Write((byte)obj);
                else
                if (obj is short) packet.Write((short)obj);
                else
                if (obj is int) packet.Write((int)obj);
                else
                if (obj is float) packet.Write((float)obj);
            }
            return packet;
        }

        ///<summary>
        /// Writes a vector2 array to an obj[] array that can be sent via netmessaging.        ///</summary>
        public static object[] WriteVector2Array(Vector2[] array)
        {
            System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
            list.Add(array.Length);
            foreach (Vector2 vec in array)
            {
                list.Add(vec.X); list.Add(vec.Y);
            }
            return list.ToArray();
        }

        ///<summary>
		/// Writes a vector2 array to a binary writer.		///</summary>
        public static void WriteVector2Array(Vector2[] array, BinaryWriter writer)
        {
            writer.Write(array.Length);
            foreach (Vector2 vec in array)
            {
                writer.Write(vec.X); writer.Write(vec.Y);
            }
        }

        ///<summary>
		/// Reads a vector2 array from a binary reader.		///</summary>
        public static Vector2[] ReadVector2Array(BinaryReader reader)
        {
            int arrayLength = reader.ReadInt32();
            Vector2[] array = new Vector2[arrayLength];
            for (int m = 0; m < arrayLength; m++)
            {
                array[m] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }
            return array;
        }
    }
}