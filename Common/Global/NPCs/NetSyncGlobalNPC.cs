using Macrocosm.Common.Netcode;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

            npc.ModNPC.NetRead(binaryReader, bitReader);
        }
    }
}
