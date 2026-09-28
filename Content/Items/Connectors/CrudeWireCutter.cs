using Macrocosm.Common.ID;
using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

/// <summary> Cuts wires (all four colors) but does not remove actuators or pipes. </summary>
public class CrudeWireCutter : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        Item.mech = true;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override bool CanUseItem(Player player) => player.ItemInTileReach(Item) && player.CanDoWireStuffHere(Player.tileTargetX, Player.tileTargetY);

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        int x = Player.tileTargetX;
        int y = Player.tileTargetY;

        // Priority order mirrors vanilla WireCutter, skips actuators and pipes
        if (WorldGen.KillWire4(x, y)) { NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, WireActionID.KillWireYellow, x, y); return true; }
        if (WorldGen.KillWire3(x, y)) { NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, WireActionID.KillWireGreen,  x, y); return true; }
        if (WorldGen.KillWire2(x, y)) { NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, WireActionID.KillWireBlue,   x, y); return true; }
        if (WorldGen.KillWire (x, y)) { NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, WireActionID.KillWireRed,    x, y); return true; }

        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CopperBar, 8)
            .AddTile(TileID.Anvils)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.TinBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
