using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Summon.Sentries;

public class MoonChampionSentry : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Summon;
        Item.damage = 300;
        Item.width = 96;
        Item.height = 96;
        Item.value = Item.sellPrice(gold: 30);
        Item.rare = ModContent.RarityType<MoonRarity2>();
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.mana = 10;
        Item.UseSound = SoundID.Item1;

        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Summon.Sentries.MoonChampionSentry>();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        position = Main.MouseWorld;
        if (!Collision.SolidTiles(position, 42, 46))
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), position, velocity, type, damage, 0f);
            player.UpdateMaxTurrets();
        }

        return false;
    }
}