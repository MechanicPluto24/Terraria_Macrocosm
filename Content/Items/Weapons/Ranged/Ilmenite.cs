using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged;

public class Ilmenite : ModItem
{
    public override void SetStaticDefaults()
    {
        Redemption.AddElement(Item, Redemption.ElementID.Arcane);
        Redemption.AddElement(Item, Redemption.ElementID.Holy);
        Redemption.AddElement(Item, Redemption.ElementID.Celestial, true);
    }

    public override void SetDefaults()
    {
        Item.DefaultToBow(30, 30, true);
        Item.damage = 450;
        Item.knockBack = 4;
        Item.width = 32;
        Item.height = 74;
        Item.value = 10000;
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.channel = true;
        Item.UseSound = null;
        Item.noUseGraphic = true;
    }

    public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] == 1 || (player.itemTime == 0 && !player.AltFunction());
    public override bool AltFunctionUse(Player player) => true;
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<IlmeniteHeld>(), damage, knockback, player.whoAmI);
        return false;
    }
}
