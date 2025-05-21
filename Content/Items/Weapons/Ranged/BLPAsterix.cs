using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class BLPAsterix : GunHeldProjectileItem
    {
        public override void SetStaticDefaults()
        {
            MoRHelper.AddElement(Item, MoRHelper.Thunder, true);
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 88;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 26;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.shoot = ModContent.ProjectileType<BLPShot>();
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SFX.EnergyShot with { Volume = 0.5f };
        }

        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(28f, 6f),
            CenterYOffset = 9f,
            MuzzleOffset = 45f,
            Recoil = (4, 0.01f),
            RecoilDiminish = 0.8f
        };

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.Item75 with { Volume = 0.7f }, player.position);

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile heldProjectile)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;

            position.Y += 2f;
            position += Main.rand.NextVector2Circular(4, 4);

            velocity = velocity.RotatedByRandom(0.05f);
        }
    }
}
