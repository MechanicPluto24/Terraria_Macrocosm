using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Buffs;
using Terraria.Audio;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Sounds;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class MetalCutterProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;

            Projectile.aiStyle = -1;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;


            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        private Player Player => Main.player[Projectile.owner];
        private int SwingDirection = 1;

        private bool despawn;
        float transparency = 1f;
        private bool spawned = false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition() => false;
        int Timer=0;
        private ProjectileAudioTracker tracker;

        public override void AI()
        {
            if(Main.MouseWorld.X>Player.Center.X)
                SwingDirection = 1;
            else
                SwingDirection = -1;
    
            if (!spawned)
            {
                Projectile.netUpdate = true;
                spawned = true;
            }

            Timer++;
            if(Timer>60)
                Projectile.frame = Timer%6 <3 ? 1 : 2;
            else
                Projectile.frame = 0;

            if(Timer>60)
            {
                Lighting.AddLight(Projectile.Center, new Color(130, 200, 255).ToVector3() * Main.rand.NextFloat(1.5f,2f));
            }
            if (!despawn)
            {
                Projectile.timeLeft = 2;
            }


            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Player.heldProj = Projectile.whoAmI;
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(-16, SwingDirection == 1 ? -10 : -20).RotatedBy(Projectile.rotation);
            Player.direction=SwingDirection;
            if (Player.noItems || !Player.channel || Player.CCed || Player.HeldItem.type != ModContent.ItemType<MetalCutter>())
            {
                despawn = true;
                return;
            }

     

            if (transparency < 1f)
                transparency += 0.005f;
        
            Projectile.velocity+=(Main.MouseWorld-Player.Center).SafeNormalize(Vector2.UnitX)*0.2f;
            Projectile.velocity=Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.rotation = Projectile.velocity.ToRotation();
        

            if(Timer>60){
                float amp = Main.rand.NextFloat(0, 1f);
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(12, 20);
                Vector2 velocity = (Utility.PolarVector(36 , MathHelper.WrapAngle(Projectile.rotation)) - Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotatedByRandom(MathHelper.PiOver4) + Player.velocity;

                Particle.Create<EngineSpark>(p =>
                {
                    p.Position = position;
                    p.Velocity = velocity;
                    p.Scale = new(Main.rand.NextFloat(1.2f, 1.8f) );
                    p.Rotation = Projectile.rotation;
                    p.ColorOnSpawn = Color.White;
                    p.ColorOnDespawn = new Color(89, 151, 193);
                });
            
                tracker ??= new(Projectile);

                SoundEngine.PlaySound(SFX.HandheldThrusterFlame with
                {
                    Volume = 0.3f,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Projectile.position, updateCallback: (sound) =>
                {
                    sound.Position = Projectile.position;
                    return tracker.IsActiveAndInGame();
                });
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(NPCSets.Material[target.type]==NPCMaterial.Metal || NPCSets.Material[target.type]==NPCMaterial.Machine)
                target.AddBuff(ModContent.BuffType<Melting>(), 240);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var rotation = Projectile.rotation;
            var origin = Projectile.Size / 2;

            Player player = Main.player[Projectile.owner];
            Vector2 position = player.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glowtexture = ModContent.Request<Texture2D>(Texture + "_glow").Value;
            Vector2 Origin = glowtexture.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale * 1.3f;
            SpriteEffects spriteEffects = ((SwingDirection==1) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.

       
            Main.EntitySpriteDraw(
               TextureAssets.Projectile[Type].Value,
               Projectile.Center + Projectile.rotation.ToRotationVector2() * 5f - Main.screenPosition,
               texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame),
               lightColor,
               rotation,
               origin,
               Projectile.scale,
               SwingDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
               0
           );
           Main.EntitySpriteDraw(
               glowtexture,
               Projectile.Center + Projectile.rotation.ToRotationVector2() * 5f - Main.screenPosition,
               glowtexture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame),
               new Color(255,255,255),
               rotation,
               origin,
               Projectile.scale,
               SwingDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
               0
           );

            return false;

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if(Timer<60)
                return false;
            float _ = 0;
            Vector2 hitboxEnd = Projectile.Center + ((Projectile.rotation)).ToRotationVector2() * (240);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitboxEnd, 30f, ref _);
        }
    }
}