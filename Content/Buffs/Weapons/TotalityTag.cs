using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Weapons
{
    public class TotalityTag : ComplexBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;

            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            DustEffects(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.dontTakeDamage)
                DustEffects(npc);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.DamageType == DamageClass.Summon)
            {
                for (int i = 0; i < 2; i++)
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

                for (float f = 0f; f < 1f; f += 1f / 4f)
                {
                    Vector2 velocity = new Vector2(3f).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                    Dust dust = Dust.NewDustPerfect(npc.Center, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.4f, 0.6f));
                    dust.noGravity = true;
                    dust.color = color.WithLuminance(0.1f);
                    dust.alpha = 15;
                }

                npc.RemoveBuff(Type);
            }
        }

        private void DustEffects(Entity entity)
        {
            int count = Math.Max(entity.width, entity.height) / 25;
            for (int i = 0; i < count; i++)
            {
                float rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                Color color = new List<Color>() {
                        new(44, 210, 91),
                        new(201, 125, 205),
                        new(114, 111, 207)
                    }.GetRandom();
                color.A = (byte)Main.rand.Next(120, 200);

                Vector2 position = entity.Center + Main.rand.NextVector2Circular(entity.width, entity.height) * 0.75f + entity.velocity * 8f;
                Particle.Create<TintableSlash>((p) =>
                {
                    p.Position = position;
                    p.Velocity = new Vector2(-0.4f * count, 0).RotatedBy((position - entity.Center).ToRotation());
                    p.Rotation = (position - entity.Center).ToRotation() - MathHelper.Pi;
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
}
