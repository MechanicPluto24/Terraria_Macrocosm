using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class ChandriumSparkle : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;

        public override int TrailCacheLenght => 3;

        public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

        [NetSync] public byte Owner;

        public Player OwnerPlayer => Main.player[Owner];

        public override int SpawnTimeLeft => 5 * 60;

        private bool whipActive;

        public override void OnSpawn()
        {
        }

        private SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            state.SaveState(spriteBatch);

            Texture2D tex = TextureAssets.Extra[89].Value;

            spriteBatch.Draw(tex, Position - screenPosition, null, new Color(177, 107, 219, 127), 0f + Rotation, TextureAssets.Extra[89].Size() / 2f, ScaleV, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, Position - screenPosition, null, new Color(177, 107, 219, 127), MathHelper.PiOver2 + Rotation, TextureAssets.Extra[89].Size() / 2f, ScaleV, SpriteEffects.None, 0f);
            Lighting.AddLight(Position, new Vector3(0.607f, 0.258f, 0.847f));

            for (int i = 1; i < TrailCacheLenght; i++)
            {
                float factor = 1f - (i / (float)TrailCacheLenght);

                spriteBatch.Draw(tex, Vector2.Lerp(OldPositions[i - 1], OldPositions[i], factor) - screenPosition, null, new Color(177, 107, 219, (int)(127 * factor)), 0f + OldRotations[i], TextureAssets.Extra[89].Size() / 2f, ScaleV * factor, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, Vector2.Lerp(OldPositions[i - 1], OldPositions[i], factor) - screenPosition, null, new Color(177, 107, 219, (int)(127 * factor)), MathHelper.PiOver2 + OldRotations[i], TextureAssets.Extra[89].Size() / 2f, ScaleV * factor, SpriteEffects.None, 0f);
            }
        }

        public override void AI()
        {
            float speed;
            float inertia;
            int whipIdx = -1;

            Rotation += Velocity.X * 0.02f;

            int chandriumWhipType = ModContent.ProjectileType<ChandriumWhipProjectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == chandriumWhipType && proj.owner == OwnerPlayer.whoAmI)
                {
                    whipIdx = i;
                    break;
                }
            }

            whipActive = whipIdx >= 0;

            if (!OwnerPlayer.active || OwnerPlayer.dead)
            {
                Kill();
                return;
            }

            if (whipActive)
            {
                // TODO: some smooth acceleration towards the whip tip
                Position = (Main.projectile[whipIdx].ModProjectile as ChandriumWhipProjectile).WhipTipPosition;
                Scale *= 0.91f;
            }
            else
            {
                if (Scale < 0.7f)
                    Scale += 0.1f;

                Vector2 vectorToIdlePosition = OwnerPlayer.Center - Center;
                float distanceToIdlePosition = vectorToIdlePosition.Length();

                if (distanceToIdlePosition > 40f)
                {
                    speed = 26f;
                    inertia = 80f;
                }
                else
                {
                    speed = 12f;
                    inertia = 60f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Velocity = (Velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Velocity == Vector2.Zero)
                {
                    Velocity.X = -2f;
                    Velocity.Y = -0.5f;
                }
            }
        }

        public override void OnKill()
        {
            if (whipActive)
                return;

            for (int i = 0; i < 5; i++)
            {
                Vector2 position = Center;
                Dust dust = Dust.NewDustDirect(position, 32, 32, ModContent.DustType<ChandriumBrightDust>());
                dust.velocity = (Velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1f, 1.4f)).RotatedByRandom(MathHelper.TwoPi);
                dust.noLight = false;
                dust.noGravity = true;
            }
        }

    }
}
