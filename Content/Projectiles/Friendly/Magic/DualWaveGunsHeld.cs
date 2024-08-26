using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class DualWaveGunsHeld : ChargedHeldProjectile
    {
        //TODO, make better idk how, also add recoil for both guns, though idk why this isn't working...
        public override string Texture => Macrocosm.EmptyTexPath;

        public float MinCharge => MaxCharge * 0.2f;
        public ref float MaxCharge => ref Projectile.ai[0];
        public ref float AI_Timer => ref Projectile.ai[1];
        public ref float AI_Charge => ref Projectile.ai[2];

        private bool altAttackActive = false;
        private int altAttackCount = 0;
        public int fired;

        public override float CircularHoldoutOffset => 8f;

        protected override bool StillInUse => base.StillInUse || Main.mouseRight || itemUseTime > 0 || altAttackActive;

        public override bool ShouldUpdateAimRotation => true;


        public override void SetProjectileStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {

        }
        float BlueRotation=0f;
        float RedRotation=0f;

        public override void ProjectileAI()
        {
            BlueRotation-=0.07f;
            RedRotation-=0.07f;

            if(BlueRotation<0f)
                BlueRotation=0f;
            if(RedRotation<0f)
                RedRotation=0f;






            if (Player.whoAmI == Main.myPlayer)
            {
                Item currentItem = Player.CurrentItem();

                int damage = Player.GetWeaponDamage(currentItem);
                float knockback = currentItem.knockBack;
                float speed;
                int usedAmmoItemId;
                if (Main.mouseRight)
                    altAttackActive = true;
                else
                    altAttackActive = false;
                if (!altAttackActive)
                {

                    if (AI_Timer % currentItem.useTime == 0)
                    {
                      

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 28f, ModContent.ProjectileType<WaveGunLaser>(), (int)(damage/1.4), knockback, Projectile.owner);

                           
                            if (fired%2==0){
                                RedRotation+=0.3f;
                                
                            }
                            else{
                                BlueRotation+=0.3f;
                               
                            }
                            fired++;
                            
                            
                                
                                

                            
                            AI_Timer = 0;
                            SoundEngine.PlaySound(SoundID.Item11, Projectile.position);
                        
                        
                    }
                }
                else
                {
                    int altAttackRate = 34;


                    if (AI_Timer % altAttackRate == 0)
                    {



                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 34f, ModContent.ProjectileType<WaveRifleLaser>(), damage, knockback, Projectile.owner);

                    }
                }

                AI_Timer++;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (!altAttackActive)
            {
                var spriteBatch = Main.spriteBatch;
                Texture2D texture2 = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/Magic/WaveGunBlue").Value;
                Vector2 rotPoint2 = Utility.RotatingPoint(Projectile.Center, new Vector2(15, 0), Projectile.rotation);
                spriteBatch.Draw(texture2, rotPoint2 - Main.screenPosition, null, lightColor, Projectile.rotation+BlueRotation, texture2.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
                Texture2D texture1 = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/Magic/WaveGunRed").Value;
                Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(10, 0), Projectile.rotation);
                spriteBatch.Draw(texture1, rotPoint - Main.screenPosition, null, lightColor, Projectile.rotation+RedRotation, texture1.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            }
            else
            {
                var spriteBatch = Main.spriteBatch;
                Texture2D texture3 = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/WaveGunRifle").Value;
                Vector2 rotPoint2 = Utility.RotatingPoint(Projectile.Center, new Vector2(10, 0), Projectile.rotation);
                spriteBatch.Draw(texture3, rotPoint2 - Main.screenPosition, null, lightColor, Projectile.rotation, texture3.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            }
            return false;
        }



    }
}
