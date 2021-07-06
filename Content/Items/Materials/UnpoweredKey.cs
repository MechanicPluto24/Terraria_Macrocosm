using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
    public class UnpoweredKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 1;

        }
    }
}