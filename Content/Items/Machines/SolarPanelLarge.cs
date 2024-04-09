using Terraria;
using Terraria.ModLoader;

<<<<<<<< HEAD:Content/Items/Machines/SolarPanelLarge.cs
namespace Macrocosm.Content.Items.Machines
========
namespace Macrocosm.Content.Items.Furniture
>>>>>>>> master:Content/Items/Furniture/SolarPanelLarge.cs
{
    public class SolarPanelLarge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.SolarPanelLarge>());
            Item.width = 44;
            Item.height = 34;
            Item.value = 500;
        }
    }
}