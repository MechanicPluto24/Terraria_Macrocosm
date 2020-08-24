using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Items.Materials
{
    public class CinisChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A large celestial chunk of swirling energy");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = 100;
            item.rare = 1;
            // Set other item.X values here
        }

        public override void AddRecipes()
        {
            // Recipes here. See Basic Recipe Guide
        }
    }
}