using Macrocosm.Items.Materials;
using Macrocosm.Subworlds;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SubworldLibrary;

namespace Macrocosm.NPCs
{
    public class MacrocosmGlobalNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            if (Main.rand.NextFloat() <= 0.10f) // NOTE: What is this Activation Core even used for?
            {
                if (npc.type == NPCID.MoonLordCore)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<ActivationCore>());
                }
            }
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            foreach (var id in EnemyCategorization.MoonEnemies)
            {
                if (!Subworld.IsActive<Moon>())
                {
                    return;
                }
                if (Subworld.IsActive<Moon>())
                {
                    pool.Clear();
                    pool.Add(id, 1f);
                }
            }
        }
    }
}