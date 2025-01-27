using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Tiles.Ambient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Items.Consumables.BossSummons
{
    public class CraterDemonSummon : ModItem
    {
        private static Asset<Texture2D> heldTexture;

        public override void Load()
        {
            heldTexture = ModContent.Request<Texture2D>(Texture + "_Held");
        }

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

            Item.noUseGraphic = true;
            Item.CustomDrawData().CustomHeldTexture = heldTexture;
        }

        public override bool CanUseItem(Player player)
            => player.InModBiome<MoonBiome>() && !NPC.AnyNPCs(ModContent.NPCType<CraterDemon>());

        public override bool? UseItem(Player player)
        {
            Utility.SummonBossDirectlyWithMessage(player.Center - new Vector2(0f, 240f), ModContent.NPCType<CraterDemon>(), sound: SoundID.ForceRoar);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Protolith>(20)
                .AddIngredient<AlienResidue>(5)
                .AddIngredient<SpaceDust>(5)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(ModContent.TileType<IrradiatedAltar>())
                .Register();
        }
    }
}
