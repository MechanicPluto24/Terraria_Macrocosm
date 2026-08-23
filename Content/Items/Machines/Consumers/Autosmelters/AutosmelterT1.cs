using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autosmelters;

public class AutosmelterT1 : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autosmelters.AutosmelterT1>());
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GrayBrick, 10)
            .AddIngredient(ItemID.IronBar, 8)
            .AddIngredient<PrintedCircuitBoard>(4)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
