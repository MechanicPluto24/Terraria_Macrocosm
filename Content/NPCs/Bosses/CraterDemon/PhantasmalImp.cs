using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    public class PhantasmalImp : ModProjectile
    {
        public Player TargetPlayer => Main.player[(int)Projectile.ai[0]];

        public bool SpawnedFromPortal
        {
            get => Projectile.ai[1] > 0f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public int Parent
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            Redemption.AddElement(Projectile, Redemption.ElementID.Arcane);
            Redemption.AddElement(Projectile, Redemption.ElementID.Shadow);
            Redemption.AddElement(Projectile, Redemption.ElementID.Celestial);
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 74;
            Projectile.hostile = true;
            Projectile.timeLeft = 15 * 60;
            Projectile.alpha = 0;
            CooldownSlot = 1;
        }

        private bool spawned;
        private Vector2 spawnPosition;
        private float flashTimer;
        private float maxFlashTimer = 10;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            => false;

        //bool spawned = false;
        public override void AI()
        {
            if (!TargetPlayer.active || (!Main.npc[Parent].active || Main.npc[Parent].type != ModContent.NPCType<CraterDemon>()) && !SpawnedFromPortal)
                Projectile.Kill();

            if (!spawned)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchRange = (-0.5f, 0.5f) }, Projectile.Center);
                spawnPosition = Projectile.Center;
                Direction = Vector2.UnitY.RotatedByRandom(MathHelper.Pi * 2);
                spawned = true;
            }

            if (!SpawnedFromPortal)
                AI_SpawnedFromCD();
            else
                AI_SpawnedFromPortal();

            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation();

            flashTimer++;
        }

        public Vector2 Direction;
        public float Speed = 10f;
        int Counter = 0;
        public bool launched = false;

        private void AI_SpawnedFromCD()
        {
            if ((CraterDemon.AttackState)Main.npc[Parent].ai[0] == CraterDemon.AttackState.SummonPhantasmals && launched == false)
            {
                if (Speed < 12f)
                    Speed += 0.06f;

                if (Projectile.DistanceSQ(Main.npc[Parent].Center) < 80f * 80f)
                {
                    Direction = Direction.RotatedBy(-0.04f * Math.Sin(Counter / 10));
                }
                else
                {
                    Vector2 notDirection = Main.npc[Parent].Center - Projectile.Center;
                    Direction = ((Direction + (notDirection * 0.0008f)).SafeNormalize(Vector2.UnitX));
                }

                Direction = Direction.SafeNormalize(Vector2.UnitY);
                Projectile.velocity = Direction * Speed;
                Counter++;
            }
            else
            {
                launched = true;
                if (Speed < 30f)
                    Speed += 0.5f;

                if (Counter != -1)
                {
                    Counter = -1;
                    Direction = (TargetPlayer.Center - Projectile.Center).RotatedByRandom(MathHelper.Pi / 5);
                }
                Direction = Direction.SafeNormalize(Vector2.UnitY);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Direction * Speed, 0.5f);
            }
        }

        private void AI_SpawnedFromPortal()
        {
            Vector2 direction = TargetPlayer.Center - Projectile.Center;
            float distance = direction.Length();
            direction.Normalize();

            float deviation = Main.rand.NextFloat(-0.1f, 0.1f);
            direction = direction.RotatedBy(deviation);

            Vector2 adjustedDirection = Vector2.Lerp(Projectile.velocity, direction, 0.2f);
            adjustedDirection.Normalize();

            float accelerateDistance = 30f * 16;
            float speed = Projectile.velocity.Length() + (distance > accelerateDistance ? distance / accelerateDistance * 0.1f : 0f);
            Projectile.velocity = adjustedDirection * speed;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White.WithOpacity(1f);

        private SpriteBatchState state;

        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int i = 1; i < length; i++)
            {
                float progress = i / (float)length;

                float wave = MathF.Sin(((length - i) + flashTimer / 2)) * (4f + 12f * progress);
                Vector2 waveOffset = Math.Abs(Projectile.velocity.X) > Math.Abs(Projectile.velocity.Y) ? new Vector2(0, wave) : new Vector2(wave, 0);
                Vector2 drawPos = Projectile.oldPos[i];
                if (i > 0) drawPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.position, 0.2f);
                drawPos += Projectile.Size / 2f + waveOffset - Main.screenPosition;

                Color trailColor = new Color(127, 127, 127, 0) * Projectile.Opacity * Utility.QuadraticEaseIn(1f - progress) * 0.35f;
                float rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.oldRot[i] : Projectile.oldRot[i];
                float scale = Projectile.scale * 1f * (1f - progress);

                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPos, null, trailColor, rotation, Projectile.Size / 2f, (float)scale, effects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.NonPremultiplied, state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, Color.White.WithOpacity(0.7f * (1f - Projectile.alpha / 255f)), Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            if (flashTimer < maxFlashTimer)
            {
                Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Flare3").Value;
                float progress = flashTimer / maxFlashTimer;
                float scale = Projectile.scale * progress * (SpawnedFromPortal ? 1.1f : 0.8f);
                Vector2 position = SpawnedFromPortal ? spawnPosition : Projectile.position + Projectile.Size / 2f;
                float opacity = SpawnedFromPortal ? 0.4f : 1f;
                Main.EntitySpriteDraw(flare, position - Main.screenPosition, null, new Color(92, 206, 130).WithOpacity(opacity), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }

            // Strange
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}
