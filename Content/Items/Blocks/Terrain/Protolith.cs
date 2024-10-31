using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Terrain
{
    public class Protolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.Protolith>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ProtolithWall>(4)
                .Register();

            Recipe.Create(ItemID.SolarBrick, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentSolar, 1)
                .Register();

            Recipe.Create(ItemID.LunarBlockSolar, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentSolar, 1)
                .Register();

            Recipe.Create(ItemID.VortexBrick, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentVortex, 1)
                .Register();

            Recipe.Create(ItemID.LunarBlockVortex, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentVortex, 1)
                .Register();

            Recipe.Create(ItemID.NebulaBrick, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentNebula, 1)
                .Register();

            Recipe.Create(ItemID.LunarBlockNebula, 01)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentNebula, 1)
                .Register();

            Recipe.Create(ItemID.StardustBrick, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentStardust, 1)
                .Register();

            Recipe.Create(ItemID.LunarBlockStardust, 10)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentStardust, 1)
                .Register();

        }
    }
}