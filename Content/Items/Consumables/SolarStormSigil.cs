using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Tiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Systems.Flags;

namespace Macrocosm.Content.Items.Consumables
{
    public class SolarStormSigil : ModItem
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
            Item.rare = ModContent.RarityType<MoonRarity3>();
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;

            Item.noUseGraphic = true;
            Item.CustomDrawData().CustomHeldTexture = heldTexture;
        }

        public override bool CanUseItem(Player player)
            => player.InModBiome<MoonBiome>();

        public override bool? UseItem(Player player)
        {
            WorldData.Current.SolarStorm=true;
            return true;
        }

    
    }
}
