using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class CrescentScripture : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 36;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.shootSpeed = 25f;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CrescentScriptureProjectile>();
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player) => false;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<CrescentScriptureProjectile>()] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = velocity;
            Projectile.NewProjectileDirect(source, position, aim, ModContent.ProjectileType<CrescentScriptureProjectile>(), damage, knockback, player.whoAmI, ai0: player.direction * player.gravDir, ai1: Item.shootSpeed, ai2: 0f);
            return false;
        }
    }
}