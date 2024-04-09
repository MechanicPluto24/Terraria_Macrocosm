using Terraria;
using Terraria.ModLoader;

<<<<<<<< HEAD:Content/Items/Machines/SolarPanelSmall.cs
namespace Macrocosm.Content.Items.Machines
========
namespace Macrocosm.Content.Items.Furniture
>>>>>>>> master:Content/Items/Furniture/SolarPanelSmall.cs
{
    public class SolarPanelSmall : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.SolarPanelSmall>());
            Item.width = 44;
            Item.height = 34;
            Item.value = 500;
        }
    }
}