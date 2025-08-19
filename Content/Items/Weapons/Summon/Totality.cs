using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Summon;

public class Totality : ModItem
{
    public override bool MeleePrefix() { return true; }
    public override void SetStaticDefaults()
    {
        Redemption.AddElement(Item, Redemption.ElementID.Arcane);
        Redemption.AddElement(Item, Redemption.ElementID.Shadow);
        Redemption.AddElement(Item, Redemption.ElementID.Celestial, true);
    }

    public override void SetDefaults()
    {
        Item.DefaultToWhip(ModContent.ProjectileType<TotalityProjectile>(), 190, 2, 4);
        Item.width = 52;
        Item.height = 48;
        Item.rare = ModContent.RarityType<MoonRarity2>();
        Item.channel = true;
    }


}
