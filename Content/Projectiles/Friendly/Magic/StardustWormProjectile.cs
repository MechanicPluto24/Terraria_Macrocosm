using Macrocosm.Common.CrossMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{

    public class StardustWormProjectile : ModProjectile
    {
        public override string Texture => base.Texture + "_Head";
        public override void SetStaticDefaults()
        {
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Arcane);
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Celestial);
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        int segmentType = 0;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active)
            {
                List<Projectile> StardustWorms = new();

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.owner == Projectile.owner && p.type == ModContent.ProjectileType<StardustWormProjectile>() && p.active)
                        StardustWorms.Add(p);
                }
                foreach (Projectile p in StardustWorms)
                {
                    p.ai[0] -= 1f;
                }
            }
        }

        public override void AI()
        {
            //Determine the type
            int numberLessThanMe = 0;

            Player player = Main.player[Projectile.owner];
            if (player.active)
            {
                List<Projectile> worms = new();

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.owner == Projectile.owner && p.type == ModContent.ProjectileType<StardustWormProjectile>() && p.active)
                        worms.Add(p);
                }

                foreach (Projectile p in worms)
                {
                    if (p.ai[0] < Projectile.ai[0])
                        numberLessThanMe++;
                }

                if (numberLessThanMe == 0) // Head
                    segmentType = 0;
                else if (numberLessThanMe == worms.Count - 1) // Tail
                    segmentType = 2;
                else // Body
                    segmentType = 1;

                if (worms.Count == 1)
                {
                    Projectile.Kill();
                }
            }

            if (segmentType == 0)
            {
                Projectile.friendly = true;
                float distanceFromTarget = 700f;
                Vector2 targetCenter = player.position;
                bool foundTarget = false;
                if (!foundTarget)
                {
                    // This code is required either way, used for finding a target
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.CanBeChasedBy())
                        {
                            float between = Vector2.Distance(npc.Center, Projectile.Center);
                            bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                            bool inRange = between < distanceFromTarget;


                            if (((closest && inRange) || !foundTarget))
                            {
                                distanceFromTarget = between;
                                targetCenter = npc.Center;
                                foundTarget = true;
                            }
                        }
                    }
                }

                float speed = 30f;
                float inertia = 20f;
                Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                List<Projectile> stardustWorms = new();

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.owner == Projectile.owner && p.type == ModContent.ProjectileType<StardustWormProjectile>() && p.active)
                        stardustWorms.Add(p);
                }

                Projectile following = null;
                if ((int)(Projectile.ai[0] - 1f) > -1 && (int)(Projectile.ai[0] - 1f) < stardustWorms.Count)
                    following = stardustWorms[(int)(Projectile.ai[0] - 1f)];

                if (following is not null)
                {
                    // Follow behind the segment "in front" of this NPC
                    // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
                    float dirX = following.Center.X - Projectile.Center.X;
                    float dirY = following.Center.Y - Projectile.Center.Y;
                    // We then use Atan2 to get a correct rotation towards that parent NPC.
                    // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
                    Projectile.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
                    // We also get the length of the direction vector.
                    float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                    // We calculate aw new, correct distance.
                    float dist = (length - Projectile.width) / length;
                    float posX = dirX * dist;
                    float posY = dirY * dist;

                    // Reset the velocity of this NPC, because we don't want it to move on its own
                    Projectile.velocity = Vector2.Zero;
                    // And set this NPCs position accordingly to that of this NPCs parent NPC.
                    Projectile.position.X += posX;
                    Projectile.position.Y += posY;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = segmentType switch
            {
                1 => ModContent.Request<Texture2D>(base.Texture + "_Body").Value,
                2 => ModContent.Request<Texture2D>(base.Texture + "_Tail").Value,
                _ => TextureAssets.Projectile[Type].Value,
            };

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, (float)(Projectile.scale * ((Math.Sin(Main.time * 3f) * 0.2f) + 1.2f)), Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

            return false;
        }

    }
}
