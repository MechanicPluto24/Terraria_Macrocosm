using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework.Graphics;
using Macrocosm.Content.Particles;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Weapons.Melee;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class CrescentScriptureProjectile : ModProjectile
    {
    

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            Main.projFrames[Type] = 1;
        }
        public ref float Timer => ref Projectile.localAI[0];
        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];
        public ref float Alt => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            // The width and height don't really matter here because we have custom collision.
            Projectile.width = 142;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true; // A line of sight check so the projectile can't deal damage through tiles.
            Projectile.ownerHitCheckDistance = 300f; // The maximum range that the projectile can hit a target. 300 pixels is 18.75 tiles.
            Projectile.usesOwnerMeleeHitCD = true; // This will make the projectile apply the standard number of immunity frames as normal melee attacks.
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.aiStyle = -1;
            Timer=1f;
        }
        int RuneTimer;
        
        public override void AI(){
            

            // Current time that the projectile has been alive.
            Player player = Main.player[Projectile.owner];
            float progress = Timer /MaxTime; // The current time over the max time.
            float velocityRotation = Projectile.velocity.ToRotation();
            if(Alt==0f){
            float adjustedRotation = MathHelper.Pi * -SwingDirection * progress - velocityRotation - SwingDirection * MathHelper.Pi + player.fullRotation+ MathHelper.Pi;
            Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.
            if(Timer==1f)
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation<0f ? Vector2.UnitX:  -Vector2.UnitX, ModContent.ProjectileType<LuminiteWave>(), (int)(Projectile.damage / 2), 1f, Main.myPlayer);
            

            Timer*=1.14f; 
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter+(new Vector2(Projectile.width/2,0).RotatedBy(adjustedRotation))) - Projectile.velocity;
            
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,  Projectile.rotation-((MathHelper.Pi/4)+MathHelper.Pi/8));
            if (Timer >= MaxTime){
             Projectile.Kill();
            }
            }
            else{
                float adjustedRotation = MathHelper.Pi * SwingDirection * progress + velocityRotation + SwingDirection * MathHelper.Pi + player.fullRotation;
            Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.
           

            Timer*=1.1f; 
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter+(new Vector2(Projectile.width/2,0).RotatedBy(adjustedRotation))) - Projectile.velocity;
            
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,  Projectile.rotation);
            if (Timer >= MaxTime){
                Particle.CreateParticle<TintableExplosion>(p =>
                {
                    p.Position = Projectile.Center+new Vector2((Projectile.width/2),0).RotatedBy(Projectile.rotation);
                    p.DrawColor = (new Color(94, 229, 163, 200));
                    p.Scale = 1.5f;
                    p.NumberOfInnerReplicas = 8;
                    p.ReplicaScalingFactor = 0.4f;
                });

                 Particle.CreateParticle<TintableExplosion>(p =>
                {
                    p.Position = Projectile.Center+new Vector2((Projectile.width/2),0).RotatedBy(Projectile.rotation);
                    p.DrawColor = (new Color(213, 155, 148, 200));
                    p.Scale = 1.2f;
                    p.NumberOfInnerReplicas = 6;
                    p.ReplicaScalingFactor = 0.2f;
                });
                CreateBlood();

                Projectile.Kill();
  
            }
            }
            RuneTimer++;
            if (RuneTimer%4==0){
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center+new Vector2(Main.rand.Next(0,(Projectile.width/2)),0).RotatedBy(Projectile.rotation), ModContent.DustType<RunicScriptDust>()).noGravity=true;
            }
        }
        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
            Player player = Main.player[Projectile.owner];
            Vector2 position = player.Center - Main.screenPosition;
            Texture2D texture1=ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Twirl1").Value;
            Texture2D texture2=ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Twirl2").Value;
            Vector2 origin = texture1.Size() / 2f;
            float scale = Projectile.scale * 1.3f;
            SpriteEffects spriteEffects = Projectile.rotation<0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally; // Flip the sprite based on the direction it is facing.
            float progress = Timer / MaxTime;
            float lerpTime = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

            Color Colour2 = new Color(213, 155, 148,0); 
            Color Colour1 = new Color(94, 229, 163,0); 
            Color frontLightColor = new Color(150, 240, 255)*1.3f;  

            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            if(Alt==0f){
            Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime*0.4f, Projectile.rotation + MathHelper.PiOver2+Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.6f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime*0.5f, Projectile.rotation + MathHelper.PiOver2+Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.5f, spriteEffects, 0f);   
            Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime*0.3f, Projectile.rotation + MathHelper.PiOver2+Projectile.ai[0]  * 1f * (1f - progress), origin, scale*1.1f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime*0.5f, Projectile.rotation + MathHelper.PiOver2+Projectile.ai[0]  * 1f * (1f - progress), origin, scale*1f, spriteEffects, 0f);   
			Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime*0.7f, Projectile.rotation + MathHelper.PiOver2+Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.9f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime*1f, Projectile.rotation + MathHelper.PiOver2+Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.8f, spriteEffects, 0f);
            }
            else
            {
            float offset = Projectile.rotation<0f ?(MathHelper.Pi/2)+(MathHelper.Pi/4):MathHelper.Pi/4;



            Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime*0.4f, Projectile.rotation+offset +Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.6f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime*0.5f, Projectile.rotation+offset + Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.5f, spriteEffects, 0f);   
            Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime*0.3f, Projectile.rotation+offset +Projectile.ai[0]  * 1f * (1f - progress), origin, scale*1.1f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime*0.5f, Projectile.rotation+offset +Projectile.ai[0]  * 1f * (1f - progress), origin, scale*1f, spriteEffects, 0f);   
			Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime*0.7f, Projectile.rotation+offset +Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.9f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime*1f, Projectile.rotation+offset +Projectile.ai[0]  * 1f * (1f - progress), origin, scale*0.8f, spriteEffects, 0f);
            }
			


            // This draws some sparkles around the circumference of the swing.
          

            // Uncomment this line for a visual representation of the projectile's size.
            // Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, position, sourceRectangle, Color.Orange * 0.75f, 0f, origin, scale, spriteEffects);

            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public void CreateBlood(){
            Vector2 ProjSpawnPosition=Projectile.Center+new Vector2((Projectile.width/2),0).RotatedBy(Projectile.rotation);
            if (!Main.rand.NextBool(50)){
            for(int i=0;i<8;i++){
                Projectile p =Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),ProjSpawnPosition,-Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4)*Main.rand.NextFloat(10f,15f), ModContent.ProjectileType<LunarBlood>(),(int)(Projectile.damage *0.5f), 0,Projectile.owner,1f);
            }
            }
            else{
                for(int i=1;i<9;i++){
                Projectile p =Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),ProjSpawnPosition,-Vector2.UnitY.RotatedBy(((MathHelper.PiOver4/2)*(i-1))-MathHelper.PiOver2+(MathHelper.PiOver4/4))*15f, ModContent.ProjectileType<LunarBlood>(),(int)(Projectile.damage *2f), 0,Projectile.owner,(float)(i+1));
            }

            }
        }


    }
}