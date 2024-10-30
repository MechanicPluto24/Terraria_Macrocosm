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
    public class ManisolBlades : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 30;
            Item.damage = 255;
            Item.autoReuse = false;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.shoot = ModContent.ProjectileType<ManisolBladeSol>();
            Item.shootSpeed = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return (player.ownedProjectileCounts[ModContent.ProjectileType<ManisolBladeSol>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<ManisolBladeMoon>()] > 0) && player.GetItemAltUseCooldown(Type) <= 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = player.ItemUseCount(Type) % 2 == 0
                ? ModContent.ProjectileType<ManisolBladeMoon>()
                : ModContent.ProjectileType<ManisolBladeSol>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ManisolBladeSol>()] > 1 && player.ownedProjectileCounts[ModContent.ProjectileType<ManisolBladeMoon>()] > 1)
                return false;
            */

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (projectile.owner != player.whoAmI)
                    continue;

                if ((type == ModContent.ProjectileType<ManisolBladeSol>() || player.AltFunction()) && projectile.active && projectile.ModProjectile is ManisolBladeSol sol && sol.AI_State == ManisolBladeBase.ActionState.Stick)
                    sol.ForceRecall();

                if ((type == ModContent.ProjectileType<ManisolBladeMoon>() || player.AltFunction()) && projectile.active && projectile.ModProjectile is ManisolBladeMoon moon && moon.AI_State == ManisolBladeBase.ActionState.Stick)
                    moon.ForceRecall();
            }

            return !player.AltFunction();
        }
    }
}
