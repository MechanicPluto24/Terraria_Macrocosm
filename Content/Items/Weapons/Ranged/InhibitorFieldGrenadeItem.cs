using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged

{
    public class InhibitorFieldGrenadeItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;

        }

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.value = Item.sellPrice(silver: 4);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
            Item.UseSound = SoundID.Item7;
            Item.autoReuse = true;
            Item.damage = 340;
            Item.knockBack = 1f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<InhibitorFieldGrenade>();
            Item.maxStack = 9999;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<InhibitorFieldGrenade>(), damage, knockback, Main.myPlayer,ai0: 450, ai1: 200);
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);

            return null;
        }

      
    }
}
