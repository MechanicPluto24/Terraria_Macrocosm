using Macrocosm.Content.Buffs.Minions;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Sounds;

namespace Macrocosm.Content.Items.Weapons.Summon
{
    public class HummingbirdDroneRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SFX.RobotSummon;

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<HummingbirdDroneSummonBuff>();

            Item.shoot = ModContent.ProjectileType<HummingbirdDrone>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
           
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, ai1:(float)player.ownedProjectileCounts[type]);
            projectile.originalDamage = Item.damage;

            return false;
        }
    }
}
