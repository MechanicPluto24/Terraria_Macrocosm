using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic;

public class CelestialMeteorStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;

        // These just add all the element icons in the tooltips
        Redemption.AddElementToItem(Type, Redemption.ElementID.Arcane);
        Redemption.AddElementToItem(Type, Redemption.ElementID.Explosive);
        Redemption.AddElementToItem(Type, Redemption.ElementID.Fire);
        Redemption.AddElementToItem(Type, Redemption.ElementID.Thunder);
        Redemption.AddElementToItem(Type, Redemption.ElementID.Celestial);
    }

    public override void SetDefaults()
    {
        Item.damage = 500;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 8;
        Item.width = 50;
        Item.height = 50;
        Item.useTime = 30;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 5;
        Item.value = 10000;
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.UseSound = SoundID.Item20;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<SolarStaffMeteor>();
        Item.shootSpeed = 10f;
        Item.tileBoost = 50;
    }

    public override Vector2? HoldoutOrigin() => new Vector2(0, 0);

    public override void AddRecipes()
    {
    }

    public int GetMeteorType()
    {
        int integer = Main.rand.Next(0, 4);
        return integer switch
        {
            0 => ModContent.ProjectileType<SolarStaffMeteor>(),
            1 => ModContent.ProjectileType<VortexStaffMeteor>(),
            2 => ModContent.ProjectileType<NebulaStaffMeteor>(),
            3 => ModContent.ProjectileType<StardustStaffMeteor>(),
            _ => 0,
        };
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
    {
        Projectile.NewProjectileDirect(source, position, (new Vector2(Main.MouseWorld.X, player.Center.Y) - position).SafeNormalize(Vector2.UnitX) * 25f, GetMeteorType(), damage, knockBack, Main.LocalPlayer.whoAmI);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = new Vector2(Main.MouseWorld.X, player.Center.Y) + new Vector2(Main.rand.Next(-300, 300), -1000);
    }
}