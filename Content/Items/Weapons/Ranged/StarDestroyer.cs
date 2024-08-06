using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Bars;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class StarDestroyer : ModItem
    {
        public override void SetStaticDefaults()
        {
             ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        
        
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.knockBack = 4;
            Item.width = 76;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.UseSound = SoundID.Item9;
            Item.useAnimation = 10;
            Item.useTime =10;
           
			Item.noMelee = true;
            Item.useStyle=5;
            Item.useAmmo = AmmoID.FallenStar;
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.shootSpeed=30f;
            Item.autoReuse=true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanConsumeAmmo(Item ammo, Player player){
            if (!player.AltFunction())
                Item.useAmmo = AmmoID.FallenStar;
            else
                Item.useAmmo = AmmoID.Bullet;
            return true;

        }
        float StarType=0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.AltFunction()){
            if (StarType==0f)
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.4), ModContent.ProjectileType<StarDestroyerStarBlue>(), damage, knockback, player.whoAmI);
            if (StarType==1f)
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.4), ModContent.ProjectileType<StarDestroyerStarYellow>(), damage, knockback, player.whoAmI);
            StarType++;
            if (StarType>1f)
                StarType=0f;
            }
            else{
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.001)*Main.rand.NextFloat(0.5f,2.0f), ModContent.ProjectileType<StarDestroyerBeam>(), damage/2, knockback, player.whoAmI);
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.001)*Main.rand.NextFloat(0.5f,2.0f), ModContent.ProjectileType<StarDestroyerBeam>(), damage/2, knockback, player.whoAmI);
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.001)*Main.rand.NextFloat(0.5f,2.0f), ModContent.ProjectileType<StarDestroyerBeam>(), damage/2, knockback, player.whoAmI);
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.001)*Main.rand.NextFloat(0.5f,2.0f), ModContent.ProjectileType<StarDestroyerBeam>(), damage/2, knockback, player.whoAmI);
            }
            return false;  
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

     

       
    }
}
