using Macrocosm.Common.Sets;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class Schroteri : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
            ItemSets.UnobtainableItem[Type] = true;
        }

        public const int MaxStacks = 20;
        public int HitStacks { get; set; }
        public int ResetTimer { get; set; }

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.value = Item.sellPrice(gold: 1);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 13;
            Item.shootsEveryUse = true;
            Item.useTime = 13;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.damage = 200;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<SchroteriProjectile>();
        }

        public override void UpdateInventory(Player player)
        {
            if (ResetTimer >= 500)
            {
                HitStacks = 0;
            }
            else
            {
                ResetTimer++;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SchroteriProjectile>()] < 1)
            {
                Item.useAnimation = 8;
                Item.useTime = 12;
                Item.useStyle = ItemUseStyleID.Shoot;
                return true;
            }

            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SchroteriProjectile>(), damage, knockback, Main.myPlayer, ai0: 0f);
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
        }
    }
}
