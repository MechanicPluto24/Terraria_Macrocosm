using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.BossSummons
{
    //Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
    public class CraterDemonSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 18;
            Item.scale = 1f;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
            => player.InModBiome<MoonBiome>() && NPC.downedMoonlord && !NPC.AnyNPCs(ModContent.NPCType<CraterDemon>());

        public override bool? UseItem(Player player)
        {
            if (Utility.SummonBossDirectlyWithMessage(player.Center - new Vector2(0f, 240f), ModContent.NPCType<CraterDemon>()))
                SoundEngine.PlaySound(SoundID.ForceRoar, player.position);

            return true;
        }
    }
}
