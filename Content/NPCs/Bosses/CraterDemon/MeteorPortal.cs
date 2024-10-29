using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    //Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
    public class MeteorPortal : ModProjectile
    {

        public ref float AITimer => ref Projectile.ai[0];
        public bool Phase2
        {
            get => Projectile.ai[1] > 0f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public int SpawnPeriod => 14;

        //Portal spawning leadup + time portals are active before they shrink
        public const int PortalTimerMax = (int)(4f * 60 + 1.5f * 60 + 24);

        private int defWidth;
        private int defHeight;

        private bool spawned;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            defWidth = defHeight = Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = PortalTimerMax;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                AITimer = 255f;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item78, Projectile.Center);
            }

            Projectile.rotation -= MathHelper.ToRadians(7.4f);

            Lighting.AddLight(Projectile.Center, new Color(182, 79, 21).ToVector3() * 1.4f * (1f - Projectile.alpha / 255f));

            if (Projectile.timeLeft >= PortalTimerMax - 90)
                AITimer -= 4f;
            else if (Projectile.timeLeft <= 90)
                AITimer += 2.83333325f;
            else
            {
                AITimer = 0f;
                if (Projectile.timeLeft % SpawnPeriod == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<FlamingMeteor>());
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                float progress = (1f - AITimer / 255f);
                Particle.Create<PortalSwirl>(p =>
                {
                    p.Position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * 2.2f * progress;
                    p.Velocity = Vector2.One * 8;
                    p.Scale = new((0.1f + Main.rand.NextFloat(0.1f)) * progress);
                    p.Color = Main.rand.NextBool() ? new Color(92, 206, 130) : new Color(182, 79, 21);
                    p.TargetCenter = Projectile.Center;
                });
            }

            Projectile.alpha = (int)MathHelper.Clamp((int)AITimer, 0f, 255f);

            Vector2 center = Projectile.Center;
            Projectile.scale = 0.05f + 0.95f * (1f - Projectile.alpha / 255f);
            Projectile.width = (int)(defWidth * Projectile.scale);
            Projectile.height = (int)(defHeight * Projectile.scale);
            Projectile.Center = center;
        }

        public override Color? GetAlpha(Color lightColor)
            => Color.White * (1f - Projectile.alpha / 255f);

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color color = Color.White;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 1f).WithOpacity(0.5f - 0.5f * Projectile.alpha / 255f), (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 1.4f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 0.84f).WithOpacity(0.5f - 0.5f * Projectile.alpha / 255f), Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * 1f, (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare3").Value;
            float scale = Projectile.scale * Main.rand.NextFloat(0.9f, 1.1f);
            Main.spriteBatch.Draw(flare, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(182, 79, 21), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            return false;
        }
    }
}
