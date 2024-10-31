using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class WornLunarianDagger : ModItem
    {
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<WornLunarianDaggerHeld>(), 18, 20f, true);

            Item.damage = 240;
            Item.knockBack = 4;
            Item.mana = 8;

            Item.width = 32;
            Item.height = 54;

            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();

            Item.channel = true;
            Item.UseSound = null;

            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] == 1 || (player.itemTime == 0 && !player.AltFunction());
        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float minCharge = 90f * player.GetAttackSpeed(DamageClass.Magic);
            Vector2 aim = velocity;
            Projectile.NewProjectileDirect(source, position, aim, ModContent.ProjectileType<WornLunarianDaggerHeld>(), damage, knockback, player.whoAmI, minCharge);
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override Vector2? HoldoutOffset()
        {
            return default;
        }
    }
}