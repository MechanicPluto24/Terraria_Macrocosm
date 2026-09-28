using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

[LegacyName("Conveyor")]
public class Pipe : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.buyPrice(copper: 10);
        Item.consumable = true;
        Item.mech = true;
        Item.ammo = Type;
    }
}
