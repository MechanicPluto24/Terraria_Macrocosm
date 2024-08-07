using Macrocosm.Content.Items.Materials.Bars;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class FrigorianGaze : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 700;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 40;
            Item.height = 30;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.UseSound = SoundID.Item9;
            Item.shoot = ModContent.ProjectileType<DianitePortal>();
            Item.noUseGraphic = true;
            Item.shootSpeed=10f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<FrigorianGazeProjectile>(), damage, knockback, player.whoAmI);
               
            
            return false;  
        }

       
			
    }
}
