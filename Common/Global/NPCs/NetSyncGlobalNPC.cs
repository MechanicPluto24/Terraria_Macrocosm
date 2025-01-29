using System.IO;
using Terraria.ModLoader.IO;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Netcode;

namespace Macrocosm.Common.Global.NPCs
{
    public class NetSyncGlobalNPC : GlobalNPC
    {
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.ModNPC is null)
                return;

            npc.ModNPC.NetWrite(binaryWriter, bitWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.ModNPC is null)
                return;

            npc.ModNPC.NetReadFields(binaryReader, bitReader);
        }
    }
}
