using System;
using System.IO;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Macrocosm;

namespace Macrocosm
{
    public class BaseNet
    {
        //------------------------------------------------------//
        //--------------------BASE NET CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods relating to netmessages.            //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//
		
		public static void SendData(int dataType, int dataA, int dataB, string text, int playerID, float dataC, float dataD, float dataE, int clientType)
		{
			NetMessage.SendData(dataType, dataA, dataB, NetworkText.FromLiteral(text), playerID, dataC, dataD, dataE, clientType);
		}

        public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
        {
            packet.Write((byte)msg);
            for (int m = 0; m < param.Length; m++)
            {
                object obj = param[m];

				if(obj is byte[])
				{
					byte[] array = (byte[])obj;
					foreach(byte b in array) packet.Write((byte)b); 
				}else
                if (obj is bool) packet.Write((bool)obj); else
                if (obj is byte) packet.Write((byte)obj); else
                if (obj is short) packet.Write((short)obj); else
                if (obj is int) packet.Write((int)obj); else
                if (obj is float) packet.Write((float)obj);
            }
            return packet;
        }		

        public static void SyncAI(Entity codable, float[] ai, int aitype)
        {
            int entType = (codable is NPC ? 0 : codable is Projectile ? 1 : -1);
            if(entType == -1){ return; }
            int id = (codable is NPC ? ((NPC)codable).whoAmI : ((Projectile)codable).identity);
            SyncAI(entType, id, ai, aitype);
        }

        /*
         * Used to sync custom ai float arrays. (the npc or Projectile requires a method called 'public void SetAI(float[] ai, int type)' that sets the ai for this to work)
         */
        public static void SyncAI(int entType, int id, float[] ai, int aitype)
        {
            object[] ai2 = new object[ai.Length + 4];
            ai2[0] = (byte)entType;
            ai2[1] = (short)id;
            ai2[2] = (byte)aitype;
            ai2[3] = (byte)ai.Length;
            for(int m = 4; m < ai2.Length; m++){ ai2[m] = ai[m - 4]; }
            MNet.SendBaseNetMessage(1, ai2);
        }

        /*
         * Writes a vector2 array to an obj[] array that can be sent via netmessaging.
         */
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

        /*
         * Writes a vector2 array to a binary writer.
         */
        public static void WriteVector2Array(Vector2[] array, BinaryWriter writer)
        {
            writer.Write(array.Length);
            foreach (Vector2 vec in array)
            {
                writer.Write(vec.X); writer.Write(vec.Y);
            }
        }

        /*
         * Reads a vector2 array from a binary reader.
         */
        public static Vector2[] ReadVector2Array(BinaryReader reader)
        {
            int arrayLength = reader.ReadInt();
            Vector2[] array = new Vector2[arrayLength];
            for (int m = 0; m < arrayLength; m++)
            {
                array[m] = new Vector2(reader.ReadFloat(), reader.ReadFloat());
            }
            return array;
        }
    }
}