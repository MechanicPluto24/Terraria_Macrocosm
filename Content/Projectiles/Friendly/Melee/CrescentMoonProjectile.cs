using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    //A fair amount of this is adapted from Ricochet Bullet
    public class CrescentMoonProjectile : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<CrescentMoon>().Texture;

        private int ricochetCounter = 0;
        private float speed = 30f;
        private int timer = 0;

        private readonly bool[] hitList = new bool[Main.maxNPCs];

        // Needed since OnHitEffect is only called on the owner
        private bool nothingToRicochet = false;
        private int newTarget = -1;

        private bool HasNewTarget => newTarget != -1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            Projectile.velocity *= speed;
            Projectile.rotation += timer > 60 ? -0.65f : 0.65f;

            if (((timer > 45 || ricochetCounter > 3) || nothingToRicochet) && speed > -45f)
                speed -= 2f;

            if (speed < 0f)
            {
                Projectile.velocity = (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Projectile.velocity *= -speed;

                if (Projectile.Distance(Main.player[Projectile.owner].Center) < 50f)
                    Projectile.Kill();
            }

            timer++;

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
        }

        // Only called on the owner 
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitList[target.whoAmI] = true; //Make sure the projectile won't aim directly for this NPC
            newTarget = GetTarget(600, Projectile.Center); //Keeps track of the current target, set to -1 to ensure no NPC by default

            bool didRicochet = HasNewTarget && ricochetCounter < 4;
            if (!HasNewTarget)
                nothingToRicochet = true;

            if (didRicochet)
            {

                ricochetCounter++;
                Vector2 shootVel = Main.npc[newTarget].Center - Projectile.Center;
                shootVel.Normalize();
                shootVel *= speed;
                Projectile.velocity = shootVel;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    Projectile.netUpdate = true;
                }
            }
        }

        private int GetTarget(float maxRange, Vector2 shootingSpot) //Function to find a NPC to target
        {
            int first = -1; //Used to keep track of the closest NPC, rather than the first one based on NPC whoAmI
            for (int j = 0; j < Main.maxNPCs; j++)
            {
                NPC npc = Main.npc[j];
                if (npc.CanBeChasedBy(this, false) && !hitList[j])
                {
                    float distance = Vector2.Distance(shootingSpot, npc.Center);
                    if (distance <= maxRange)
                    {
                        if ((first == -1 || distance < Vector2.Distance(shootingSpot, Main.npc[first].Center)) && Collision.CanHitLine(shootingSpot, 0, 0, npc.Center, 0, 0))
                            first = j;
                    }
                }
            }

            return first;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            for (int i = 1; i < length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;

                Color trailColor = Color.White * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPos, null, trailColor, (float)(Main.time + (i * 3)), Projectile.Size / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, Color.White.WithOpacity(0.7f * (1f - Projectile.alpha / 255f)), Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}