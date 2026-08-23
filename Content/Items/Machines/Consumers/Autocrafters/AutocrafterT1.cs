using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autocrafters;

public class AutocrafterT1 : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autocrafters.AutocrafterT1>());
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.IronBar, 10)
            .AddIngredient(ItemID.CopperBar, 8)
            .AddIngredient<PrintedCircuitBoard>(6)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
