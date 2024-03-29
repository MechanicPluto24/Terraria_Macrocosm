using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Blocks
{
    public class SilicaCrimsand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
            ItemID.Sets.ExtractinatorMode[Type] = ModContent.ItemType<SilicaSand>();
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Blocks.SilicaCrimsand>();

            // TODO: Using this Sand in the Sandgun would require PickAmmo code and changes to the falling sand projectile or a new ModProjectile.
            // TML: This could also work as an ExampleSand contribution (to check if someone else did it already in the meantime)
            //Item.ammo = AmmoID.Sand;
        }


        public override void AddRecipes()
        {

        }
    }
}