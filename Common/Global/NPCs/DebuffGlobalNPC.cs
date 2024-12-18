using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Common.Global.NPCs
{

    public class DebuffGlobalNPC : GlobalNPC
    {
        public override void Load()
        {
        }

        public override void PostAI(NPC npc)
        {
            if (npc.HasBuff<ProcellarumLightningMarkDebuff>())
            {
                for (int i = 0; i < 3; i++)
                {
                    float rotation = Main.rand.NextFloat() * MathHelper.TwoPi + Main.rand.NextFloatDirection() * 0.25f;
                    Particle.Create<LightningParticle>((p) =>
                    {
                        p.Position = npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height) * 0.75f + npc.velocity * 4f;
                        p.Rotation = (npc.Center - p.Position).ToRotation();
                        p.Color = new Color(232, 243, 255, 255);
                        p.OutlineColor = (Main.rand.NextBool() ? new Color(156, 174, 208, 127) : new Color(179, 171, 185, 127)) * 0.5f;
                        p.Scale = new(Main.rand.NextFloat(0.4f, 0.8f));
                        p.Velocity = Main.rand.NextVector2Circular(0.1f, 0.1f);
                        p.Scale = new Vector2(1f, 1f) * Main.rand.NextFloat(0.1f, 0.6f);
                        p.ScaleVelocity = new Vector2(-0.05f);
                        p.TimeToLive = 10;
                        p.FadeInNormalizedTime = 0.01f;
                        p.FadeOutNormalizedTime = 0.5f;

                    });
                }
            }

            if (npc.HasBuff<TotalitySlashed>())
            {
                int count = Math.Max(npc.width, npc.height) / 25;
                for (int i = 0; i < count; i++)
                {
                    float rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                    Color color = new List<Color>() {
                        new(44, 210, 91),
                        new(201, 125, 205),
                        new(114, 111, 207)
                    }.GetRandom();
                    color.A = (byte)Main.rand.Next(120, 200);

                    Vector2 position = npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height) * 0.75f + npc.velocity * 8f;
                    Particle.Create<TintableSlash>((p) =>
                    {
                        p.Position = position ;
                        p.Velocity = new Vector2(-0.4f * count, 0).RotatedBy((position - npc.Center).ToRotation());
                        p.Rotation = (position - npc.Center).ToRotation() - MathHelper.Pi;
                        p.Color = color;
                        p.SecondaryColor = (color * 2f).WithOpacity(0.2f);
                        p.FrameSpeed = 2;
                        p.Scale = new Vector2(0.3f, 0.4f) * Main.rand.NextFloat(0.2f, 0.7f);
                        p.ScaleVelocity = new Vector2(-0.01f, -0.01f);
                        p.FadeInNormalizedTime = 0.01f;
                        p.FadeOutNormalizedTime = 0.5f;
                    });
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<ProcellarumLightningMarkDebuff>() && projectile.type == ModContent.ProjectileType<ProcellarumHalberdProjectile>())
            {
                modifiers.FinalDamage *= 1.5f;
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<ProcellarumHalberdProjectile>())
            {
                npc.RemoveBuff<ProcellarumLightningMarkDebuff>();
            }

            if (npc.HasBuff<TotalitySlashed>() && projectile.minion)
            {
                for(int i = 0; i < 2; i++)
                {
                    Vector2 position = npc.Center + Main.rand.NextVector2CircularEdge(npc.width, npc.height);
                    Vector2 velocity = new Vector2(5).RotatedByRandom(MathHelper.TwoPi);

                    float rotation = (Main.rand.Next(2) * 2 - 1) * ((float)Math.PI / 5f + (float)Math.PI * 4f / 5f * Main.rand.NextFloat()) * 0.5f;
                    velocity = velocity.RotatedBy(MathHelper.PiOver4);

                    position += npc.velocity * 5;
                    Projectile.NewProjectile(projectile.GetSource_OnHit(npc), position, velocity, ModContent.ProjectileType<TotalitySmallSlashProjectile>(), (int)(damageDone * 0.25f), 0f, projectile.owner, rotation);
                }

                Color color = new List<Color>() {
                        new(44, 210, 91),
                        new(201, 125, 205),
                        new(114, 111, 207)
                    }.GetRandom();
                color.A = (byte)Main.rand.Next(120, 200);
                Particle.Create<TintableSlash>((p) =>
                {
                    p.Position = npc.Center;
                    p.Velocity = Vector2.Zero;
                    p.Rotation = (npc.Center - projectile.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8);
                    p.Color = color;
                    p.SecondaryColor = (color * 2f).WithOpacity(0.2f);
                    p.FrameSpeed = 2;
                    p.Scale = new(Main.rand.NextFloat(0.5f, 0.8f), Main.rand.NextFloat(0.8f, 1.2f));
                    p.ScaleVelocity = new Vector2(0.01f);
                    p.FadeInNormalizedTime = 0.01f;
                    p.FadeOutNormalizedTime = 0.5f;
                });


                npc.RemoveBuff<TotalitySlashed>();
            }
        }
    }
}
