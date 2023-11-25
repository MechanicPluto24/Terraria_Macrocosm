using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
    public class SpaceDust : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.value = 100;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.material = true;

            // Set other Item.X values here
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.timeForVisualEffects % 15 == 0)
                Dust.NewDustPerfect(Item.Center + Main.rand.NextVector2Circular(Item.width * 0.6f, Item.height * 0.6f), DustID.SilverCoin, new Microsoft.Xna.Framework.Vector2(0f, -0.25f), Scale: Main.rand.NextFloat(0.05f, 0.1f));
        }

        public override void AddRecipes()
        {
            // Recipes here. See Basic Recipe Guide
        }
    }
}