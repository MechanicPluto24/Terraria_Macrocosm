using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using SDL2;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class ManisolBlades : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 38;
            Item.damage = 500;
            Item.autoReuse = false;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.shoot = ModContent.ProjectileType<ManisolBladeSol>();
            Item.shootSpeed = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.AltFunction())
                type = ModContent.ProjectileType<ManisolBladeMoon>();
            else
                type = ModContent.ProjectileType<ManisolBladeSol>();
        }

        /*public void CheckShoot(Player player, int type)
        {
            int count = 0;
            List<Projectile> list = new List<Projectile>();
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.owner == player.whoAmI
                && (projectile.ModProjectile is type))
                {
                    Main.projectile[i].ai[1] = 2;
                    Main.projectile[i].velocity = new Vector2(10, 10);
                    count++;
                    if (count >= 2)
                        break;
                }
            }
        }*/

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.AltFunction())
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.owner == player.whoAmI && projectile.active && projectile.ai[1] != 2
                    && (projectile.ModProjectile is ManisolBladeMoon moon))
                    {
                        projectile.ai[1] = 2;
                        projectile.velocity = new Vector2(10, 10);
                        projectile.timeLeft = 300; //Failsafe
                        moon.OnReturn();
                        return false;
                    }
                }

                if (player.ownedProjectileCounts[ModContent.ProjectileType<ManisolBladeMoon>()] >= 1)
                    return false;

                return true;
            }
            else
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.owner == player.whoAmI && projectile.active && projectile.ai[1] != 2
                    && (projectile.ModProjectile is ManisolBladeSol sol))
                    {
                        projectile.ai[1] = 2;
                        projectile.velocity = new Vector2(10, 10);
                        projectile.timeLeft = 300;
                        sol.OnReturn();
                        return false;
                    }
                }

                if (player.ownedProjectileCounts[ModContent.ProjectileType<ManisolBladeSol>()] >= 1)
                    return false;

                return true;
            }
        }
    }
}
